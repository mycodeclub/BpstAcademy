namespace BpstAcademy.Domain.Common;

/// <summary>
/// Base for business tables: audit fields, soft delete and an optimistic concurrency token
/// (mapped to PostgreSQL's <c>xmin</c>, so two people saving the same record cannot silently overwrite each other).
/// </summary>
public abstract class AuditableEntity : Entity, IAuditable, ISoftDelete
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public uint Version { get; private set; }
}
