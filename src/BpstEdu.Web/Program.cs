using System.Text.Encodings.Web;
using System.Text.Unicode;
using BpstEdu.Application.Security;
using BpstEdu.Infrastructure;
using BpstEdu.Infrastructure.Persistence;
using BpstEdu.Infrastructure.Persistence.Seed;
using BpstEdu.Web.Options;
using BpstEdu.Web.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;

var builder = WebApplication.CreateBuilder(args);

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

// Opt-in local convenience for a private database (Docker): set Database:MigrateOnStartup=true in user secrets.
// Off by default so a machine pointed at a shared or live database never changes its schema by starting up;
// those get migrations as a deliberate step (dotnet ef database update / EF migration bundle).
if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
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
