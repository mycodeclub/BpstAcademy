using Microsoft.AspNetCore.Identity;

namespace BpstEdu.Infrastructure.Identity;

/// <summary>A named bundle of permissions, stored as role claims.</summary>
public class AppRole : IdentityRole<Guid>
{
    public AppRole() => Id = Guid.CreateVersion7();

    public AppRole(string name) : this() => Name = name;
}
