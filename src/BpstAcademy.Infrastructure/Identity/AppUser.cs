using Microsoft.AspNetCore.Identity;

namespace BpstAcademy.Infrastructure.Identity;

/// <summary>A portal sign-in: staff, trainer or student. Ids are GUID v7 like every other table.</summary>
public class AppUser : IdentityUser<Guid>
{
    public AppUser() => Id = Guid.CreateVersion7();
}
