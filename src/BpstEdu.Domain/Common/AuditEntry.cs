namespace BpstEdu.Domain.Common;

/// <summary>One row per created, changed or deleted record. Written automatically on save; feeds the audit log screen.</summary>
public sealed class AuditEntry
{
    public long Id { get; private set; }
    public DateTimeOffset At { get; init; }
    public string? UserId { get; init; }
    public required string EntityType { get; init; }
    public required string EntityId { get; init; }
    public required AuditAction Action { get; init; }

    /// <summary>JSON of changed properties: <c>{"Name": {"old": "A", "new": "B"}}</c>.</summary>
    public string? Changes { get; init; }
}

public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3,
}
