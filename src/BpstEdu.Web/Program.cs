using System.Text.Encodings.Web;
using System.Text.Unicode;
using BpstEdu.Application.Security;
using BpstEdu.Infrastructure;
using BpstEdu.Infrastructure.Persistence;
using BpstEdu.Web.Options;
using BpstEdu.Web.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
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

// Portal sign-in cookie. ASP.NET Core Identity and permission claims replace the demo sign-in in B1.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
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

// Local convenience only: bring the Docker database up to date on start.
// Production runs migrations as a separate deploy step (EF migration bundle), never at startup.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
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
