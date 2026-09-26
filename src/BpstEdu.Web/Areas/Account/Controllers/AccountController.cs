using BpstEdu.Infrastructure.Identity;
using BpstEdu.Web.Areas.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BpstEdu.Web.Areas.Account.Controllers;

/// <summary>Sign in / sign out for the portal. Users sign in with their email or BPST ID (the Identity user name).</summary>
[Area("Account")]
[AllowAnonymous]
public class AccountController(SignInManager<AppUser> signIn, UserManager<AppUser> users) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginInput { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInput input)
    {
        if (ModelState.IsValid)
        {
            var loginId = input.LoginId.Trim();
            var user = loginId.Contains('@') ? await users.FindByEmailAsync(loginId) : await users.FindByNameAsync(loginId);
            if (user is not null)
            {
                var result = await signIn.PasswordSignInAsync(user, input.Password, input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded) return LocalRedirectOrHome(input.ReturnUrl);
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Too many failed attempts. Try again in a few minutes.");
                    input.Password = "";
                    return View(input);
                }
            }
            ModelState.AddModelError(string.Empty, "Email/ID or password is incorrect.");
        }
        input.Password = "";
        return View(input);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Denied() => View();

    private IActionResult LocalRedirectOrHome(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Dashboard", new { area = "Me" });
}
