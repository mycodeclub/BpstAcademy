using System.Security.Claims;
using BpstAcademy.Application.Security;
using BpstAcademy.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BpstAcademy.Infrastructure.Persistence.Seed;

/// <summary>
/// The minimum a fresh database needs to be usable: the Admin role with every permission, and one admin user.
/// Idempotent: safe to run again, it only adds what is missing. Creates no sample data.
/// </summary>
public sealed class IdentitySeeder(RoleManager<AppRole> roles, UserManager<AppUser> users, ILogger<IdentitySeeder> log)
{
    public async Task SeedAsync(string adminEmail, string adminPassword)
    {
        var admin = await roles.FindByNameAsync(DefaultRoles.Admin);
        if (admin is null)
        {
            admin = new AppRole(DefaultRoles.Admin);
            Check(await roles.CreateAsync(admin));
            log.LogInformation("Created role {Role}", DefaultRoles.Admin);
        }

        var granted = (await roles.GetClaimsAsync(admin))
            .Where(c => c.Type == Permissions.ClaimType).Select(c => c.Value).ToHashSet();
        foreach (var permission in Permissions.All.Where(p => !granted.Contains(p)))
            Check(await roles.AddClaimAsync(admin, new Claim(Permissions.ClaimType, permission)));

        var user = await users.FindByEmailAsync(adminEmail);
        if (user is null)
        {
            user = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            Check(await users.CreateAsync(user, adminPassword));
            log.LogInformation("Created admin user {Email}", adminEmail);
        }

        if (!await users.IsInRoleAsync(user, DefaultRoles.Admin))
            Check(await users.AddToRoleAsync(user, DefaultRoles.Admin));
    }

    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException("Seed failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
