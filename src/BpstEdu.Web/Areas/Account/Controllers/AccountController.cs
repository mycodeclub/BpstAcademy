using System.Security.Claims;
using BpstEdu.Web.Areas.Account.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BpstEdu.Web.Areas.Account.Controllers;

/// <summary>
/// Sign in / sign out for the portal. Real sign-in (ASP.NET Core Identity, BPST IDs, permissions) arrives in B1;
/// until then only the Development-only demo sign-in works.
/// </summary>
[Area("Account")]
[AllowAnonymous]
public class AccountController(IWebHostEnvironment env) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginInput { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginInput input)
    {
        if (ModelState.IsValid)
            ModelState.AddModelError(string.Empty, "Sign-in is not available yet. It opens once user accounts are set up.");
        input.Password = "";
        return View(input);
    }

    /// <summary>Signs in a demo user with no roles or permissions so the portal layout can be seen. Development only.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DemoSignIn(string? returnUrl = null)
    {
        if (!env.IsDevelopment()) return NotFound();

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "demo"), new Claim(ClaimTypes.Name, "Demo User")],
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return LocalRedirectOrHome(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Denied() => View();

    private IActionResult LocalRedirectOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Dashboard", new { area = "Me" });
}
