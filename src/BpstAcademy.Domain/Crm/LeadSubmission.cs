using BpstAcademy.Domain.Common;

namespace BpstAcademy.Domain.Crm;

/// <summary>
/// Every accepted form post from the public website, exactly as received. Never changed or deleted, so no submission is
/// lost even if the <see cref="Lead"/> it belongs to is later edited.
/// </summary>
public class LeadSubmission : Entity
{
    public Guid LeadId { get; set; }

    /// <summary>enquiry, prebook_started or prebook_paid.</summary>
    public required string FormType { get; init; }
    public string? BookingRef { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }

    /// <summary>The posted JSON.</summary>
    public required string Payload { get; init; }
    public string? UserAgent { get; init; }
}
