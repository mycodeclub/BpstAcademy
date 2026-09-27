namespace BpstAcademy.Application.Security;

/// <summary>The signed-in user for the current request, or anonymous. Permissions and data scope are added in B1.</summary>
public interface ICurrentUser
{
    string? UserId { get; }
    bool IsAuthenticated { get; }
}
