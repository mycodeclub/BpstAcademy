namespace BpstAcademy.Domain.Common;

/// <summary>Who created / last changed a row and when (UTC). Filled in by the persistence layer, never by hand.</summary>
public interface IAuditable
{
    DateTimeOffset CreatedAt { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}
