using System.Text.Encodings.Web;
using System.Text.Unicode;
using BpstAcademy.Application.Security;
using BpstAcademy.Infrastructure;
using BpstAcademy.Infrastructure.Persistence;
using BpstAcademy.Infrastructure.Persistence.Seed;
using BpstAcademy.Web.Options;
using BpstAcademy.Web.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;

var builder = WebApplication.CreateBuilder(args);

// Git-ignored local settings in the project folder (connection strings, seed admin). User secrets live in each
// machine's profile, so a folder shared between machines (Mac + Windows VM) needs this file to carry them.
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile("appsettings.Development.local.json", optional: true, reloadOnChange: true);

builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection(SiteOptions.SectionName));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
// Emit ₹, — and Hindi text as-is instead of &#x..; entities (readable page source for search engines).
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

// Portal sign-in cookie (ASP.NET Core Identity, registered in AddInfrastructure). Role permission claims go into it at sign-in.
builder.Services.ConfigureApplicationCookie(o =>
    {
        o.LoginPath = "/portal/account/login";
        o.LogoutPath = "/portal/account/logout";
        o.AccessDeniedPath = "/portal/account/denied";
        o.Cookie.Name = "bpst.auth";
        o.Cookie.HttpOnly = true;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

var (database, connectionString) = DatabaseConnection.Resolve(app.Configuration);
app.Logger.LogInformation("Database: {Name} ({Server})", database, DatabaseConnection.Describe(connectionString));

// Local convenience for a private database: the "Docker DB" and "Local DB" launch profiles set Database:MigrateOnStartup.
// Never for the live database, which gets migrations as a deliberate step (dotnet ef database update / EF migration bundle),
// so starting the app never changes its schema.
if (app.Environment.IsDevelopment() && database != DatabaseConnection.Live
    && app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}

// `dotnet run -- seed`: create the Admin role and the admin user in the configured database, then exit.
// The admin's email and password come from Seed:AdminEmail / Seed:AdminPassword (user secrets), never from the repo.
if (args.Contains("seed"))
{
    var email = app.Configuration["Seed:AdminEmail"];
    var password = app.Configuration["Seed:AdminPassword"];
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        throw new InvalidOperationException("Set Seed:AdminEmail and Seed:AdminPassword (dotnet user-secrets) before running the seed.");

    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<IdentitySeeder>().SeedAsync(email, password);
    return;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/error/{0}");

// One public address: bpstacademy.com (no www) redirects permanently to the Site:BaseUrl host, keeping path and query.
var canonicalHost = new Uri(app.Services.GetRequiredService<IOptions<SiteOptions>>().Value.BaseUrl).Host;
if (canonicalHost.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
{
    var bareHost = canonicalHost[4..];
    app.Use(async (context, next) =>
    {
        if (!string.Equals(context.Request.Host.Host, bareHost, StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }
        var request = context.Request;
        context.Response.Redirect($"https://{canonicalHost}{request.PathBase}{request.Path}{request.QueryString}", permanent: true);
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapHealthChecks("/health");

// Portal: /portal/{area}/{controller}/{action}. Areas are business modules (Crm, Academics, Finance, ...) plus Account and Me.
// Every portal page needs a signed-in user; Account's sign-in actions opt out with [AllowAnonymous].
app.MapGet("/portal", () => Results.Redirect("/portal/me/dashboard"));
app.MapAreaControllerRoute(
        name: "portal-account",
        areaName: "Account",
        pattern: "portal/account/{action=Login}",
        defaults: new { controller = "Account" })
    .WithStaticAssets();
app.MapControllerRoute(
        name: "portal",
        pattern: "portal/{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .RequireAuthorization()
    .WithStaticAssets();

// Public website.
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/// <summary>Entry point, public for WebApplicationFactory in the integration tests.</summary>
public partial class Program;
