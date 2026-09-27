namespace BpstAcademy.Application.Security;

/// <summary>
/// Every permission the portal checks. Roles are bundles of these, stored as role claims of type <see cref="ClaimType"/>.
/// Each module adds its own permissions here as it is built.
/// </summary>
public static class Permissions
{
    public const string ClaimType = "permission";

    public const string UsersManage = "users.manage";
    public const string RolesManage = "roles.manage";

    public static IReadOnlyList<string> All { get; } = [UsersManage, RolesManage];
}
