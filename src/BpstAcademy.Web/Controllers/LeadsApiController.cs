using System.Text.Json;
using System.Text.RegularExpressions;
using BpstAcademy.Application.Abstractions;
using BpstAcademy.Domain.Crm;
using BpstAcademy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace BpstAcademy.Web.Controllers;

/// <summary>
/// Receives the public website forms (site/js/bpst.js, <c>leadEndpoint</c> in site/js/config.js): contact enquiries,
/// ₹49 pre-bookings (saved before payment) and payments the visitor reports. Every accepted post is kept in
/// <see cref="LeadSubmission"/>; the <see cref="Lead"/> row is what the CRM screens show.
/// </summary>
[ApiController]
[Route("api/leads")]
[EnableRateLimiting(RateLimitPolicy)]
[RequestSizeLimit(32 * 1024)]
[ApiExplorerSettings(IgnoreApi = true)]
public partial class LeadsApiController(AppDbContext db, IClock clock, ILogger<LeadsApiController> log) : ControllerBase
{
    public const string RateLimitPolicy = "leads";

    private const string Enquiry = "enquiry", PrebookStarted = "prebook_started", PrebookPaid = "prebook_paid";

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] JsonElement body, CancellationToken ct)
    {
        if (body.ValueKind != JsonValueKind.Object) return BadRequest(Error("Expected a JSON object."));
        if (!string.IsNullOrEmpty(Text(body, "website"))) return Ok(new { ok = true }); // honeypot: pretend success

        var formType = Text(body, "form_type");
        if (formType is not (Enquiry or PrebookStarted or PrebookPaid)) return BadRequest(Error("Unknown form_type."));

        var errors = new Dictionary<string, string>();
        var name = Clean(Text(body, "name"), 100);
        if (name is null || name.Length < 2) errors["name"] = "Enter your name.";
        var mobile = NormaliseMobile(Text(body, "mobile"));
        if (mobile is null) errors["mobile"] = "Enter a 10-digit Indian mobile number.";
        var email = Clean(Text(body, "email"), 254);
        if (email is not null && !EmailPattern().IsMatch(email)) errors["email"] = "Enter a valid email address.";
        var bookingRef = Clean(Text(body, "booking_ref"), 32);
        if (formType != Enquiry && (bookingRef is null || !BookingRefPattern().IsMatch(bookingRef))) errors["booking_ref"] = "Missing booking reference.";
        var paymentReference = Clean(Text(body, "payment_id"), 100);
        if (formType == PrebookPaid && paymentReference is null) errors["payment_id"] = "Missing payment reference.";
        if (errors.Count > 0) return ValidationProblem(new ValidationProblemDetails(errors.ToDictionary(e => e.Key, e => new[] { e.Value })));

        var now = clock.UtcNow;
        var lead = formType == Enquiry ? null : await db.Leads.FirstOrDefaultAsync(l => l.BookingRef == bookingRef, ct);
        if (lead is null)
        {
            lead = new Lead
            {
                Kind = formType == Enquiry ? LeadKind.Enquiry : LeadKind.PreBooking,
                BookingRef = formType == Enquiry ? null : bookingRef,
                Name = name!,
                Mobile = mobile!,
            };
            db.Leads.Add(lead);
        }

        lead.Name = name!;
        lead.Mobile = mobile!;
        lead.Email = email ?? lead.Email;
        lead.City = Clean(Text(body, "city"), 100) ?? lead.City;
        lead.CourseSlug = Clean(Text(body, "course"), 100) ?? lead.CourseSlug;
        lead.CourseTitle = Clean(Text(body, "course_title"), 200) ?? lead.CourseTitle;
        lead.Duration = Clean(Text(body, "duration"), 60) ?? lead.Duration;
        lead.Mode = Clean(Text(body, "mode"), 60) ?? lead.Mode;
        lead.Qualification = Clean(Text(body, "qualification"), 100) ?? lead.Qualification;
        lead.Message = Clean(Text(body, "message"), 2000) ?? lead.Message;
        lead.Consent = body.TryGetProperty("consent", out var consent) && consent.ValueKind == JsonValueKind.True;
        lead.SourcePage = Clean(Text(body, "page"), 300) ?? lead.SourcePage;
        if (body.TryGetProperty("utm", out var utm) && utm.ValueKind == JsonValueKind.Object && utm.EnumerateObject().Any())
            lead.Utm = Truncate(utm.GetRawText(), 4000);
        if (formType != Enquiry && body.TryGetProperty("amount", out var amount) && amount.TryGetDecimal(out var value) && value is > 0 and < 100_000)
            lead.Amount = value;
        if (formType == PrebookPaid)
        {
            lead.Status = LeadStatus.PaymentReported;
            lead.PaymentMethod = Clean(Text(body, "payment_method"), 20);
            lead.PaymentReference = paymentReference;
            lead.PaymentReportedAt = now;
        }

        db.LeadSubmissions.Add(new LeadSubmission
        {
            LeadId = lead.Id,
            FormType = formType,
            BookingRef = lead.BookingRef,
            ReceivedAt = now,
            Payload = body.GetRawText(),
            UserAgent = Truncate(Request.Headers.UserAgent.ToString(), 512),
        });
        await db.SaveChangesAsync(ct);

        log.LogInformation("Website {FormType} saved as lead {LeadId}", formType, lead.Id);
        return StatusCode(StatusCodes.Status201Created, new { ok = true, id = lead.Id, booking_ref = lead.BookingRef });
    }

    private static object Error(string message) => new { ok = false, error = message };

    private static string? Text(JsonElement body, string name) =>
        body.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static string? Clean(string? value, int max)
    {
        var text = value?.Trim();
        return string.IsNullOrEmpty(text) ? null : Truncate(text, max);
    }

    private static string Truncate(string value, int max) => value.Length <= max ? value : value[..max];

    /// <summary>"+91 98765 43210", "09876543210" or "9876543210" → "+919876543210"; null if not an Indian mobile.</summary>
    private static string? NormaliseMobile(string? value)
    {
        var digits = new string((value ?? "").Where(char.IsAsciiDigit).ToArray());
        if (digits.Length == 12 && digits.StartsWith("91", StringComparison.Ordinal)) digits = digits[2..];
        else if (digits.Length == 11 && digits[0] == '0') digits = digits[1..];
        return IndianMobilePattern().IsMatch(digits) ? "+91" + digits : null;
    }

    [GeneratedRegex(@"^[6-9]\d{9}$")]
    private static partial Regex IndianMobilePattern();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();

    [GeneratedRegex(@"^BPST-[A-Z0-9-]{4,27}$")]
    private static partial Regex BookingRefPattern();
}
