using System.Net;
using System.Text.RegularExpressions;
using BpstEdu.Application.Security;
using BpstEdu.Infrastructure.Identity;
using BpstEdu.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BpstEdu.IntegrationTests;

/// <summary>The seed creates a working admin, and the login form signs them in.</summary>
[Collection(AppCollection.Name)]
public partial class SignInTests(AppFactory factory) : IAsyncLifetime
{
    private const string Email = "seed-test@bpst.test";
    private const string Password = "Seed-Test-Pass1";

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await seeder.SeedAsync(Email, Password);
        await seeder.SeedAsync(Email, Password); // idempotent
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Seed_gives_the_admin_role_every_permission()
    {
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        var user = await users.FindByEmailAsync(Email);
        Assert.NotNull(user);
        Assert.Equal([DefaultRoles.Admin], await users.GetRolesAsync(user));

        var claims = await roles.GetClaimsAsync((await roles.FindByNameAsync(DefaultRoles.Admin))!);
        Assert.Equal(Permissions.All.Order(), claims.Where(c => c.Type == Permissions.ClaimType).Select(c => c.Value).Order());
    }

    [Fact]
    public async Task Admin_signs_in_and_opens_the_portal()
    {
        var client = Client();

        var response = await PostLogin(client, Email, Password);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/portal/me", response.Headers.Location!.OriginalString);
        var dashboard = await client.GetAsync("/portal/me/dashboard", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, dashboard.StatusCode);
    }

    [Fact]
    public async Task Wrong_password_stays_on_login_with_an_error()
    {
        var response = await PostLogin(Client(), Email, "not-the-password");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Email/ID or password is incorrect.", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    // The auth cookie is Secure, so the client talks https.
    private HttpClient Client() => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost"),
    });

    private static async Task<HttpResponseMessage> PostLogin(HttpClient client, string loginId, string password)
    {
        var page = await client.GetStringAsync("/portal/account/login", TestContext.Current.CancellationToken);
        var token = AntiforgeryToken().Match(page).Groups[1].Value;

        return await client.PostAsync("/portal/account/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["LoginId"] = loginId,
            ["Password"] = password,
        }), TestContext.Current.CancellationToken);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryToken();
}
