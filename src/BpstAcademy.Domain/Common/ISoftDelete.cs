namespace BpstAcademy.Domain.Common;

/// <summary>Rows are never physically deleted: a delete sets these fields and the row is hidden from normal queries.</summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
