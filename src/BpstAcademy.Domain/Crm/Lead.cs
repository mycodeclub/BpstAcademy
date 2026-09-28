using BpstAcademy.Domain.Common;

namespace BpstAcademy.Domain.Crm;

/// <summary>
/// One enquiry or ₹49 pre-booking from the public website. A pre-booking is saved as soon as the form is submitted,
/// before any payment; a payment the visitor reports (UPI transaction ID or Razorpay payment ID) is added to the same row
/// and stays unverified until someone checks it against the bank or Razorpay.
/// </summary>
public class Lead : AuditableEntity
{
    public LeadKind Kind { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;

    /// <summary>Pre-booking reference shown to the visitor, e.g. BPST-D26-7KQ2MZ. Null for plain enquiries.</summary>
    public string? BookingRef { get; set; }

    public required string Name { get; set; }

    /// <summary>+91 followed by 10 digits.</summary>
    public required string Mobile { get; set; }
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? CourseSlug { get; set; }
    public string? CourseTitle { get; set; }
    public string? Duration { get; set; }
    public string? Mode { get; set; }
    public string? Qualification { get; set; }
    public string? Message { get; set; }
    public bool Consent { get; set; }

    /// <summary>Site path the form was sent from, e.g. /courses/java-full-stack.</summary>
    public string? SourcePage { get; set; }

    /// <summary>Campaign tags (utm_*, gclid, fbclid, landing page, referrer) as JSON.</summary>
    public string? Utm { get; set; }

    public decimal? Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    public DateTimeOffset? PaymentReportedAt { get; set; }
}

public enum LeadKind
{
    Enquiry = 1,
    PreBooking = 2,
}

public enum LeadStatus
{
    New = 1,

    /// <summary>The visitor says they paid; not yet checked against the bank or Razorpay.</summary>
    PaymentReported = 2,
}
