using BpstAcademy.Application.Security;
using BpstAcademy.Domain.Crm;
using BpstAcademy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BpstAcademy.Web.Areas.Crm.Controllers;

/// <summary>Website enquiries and ₹49 pre-bookings (read-only until the full CRM lands in B3).</summary>
[Area("Crm")]
[Authorize(Policy = Permissions.LeadsView)]
public class LeadsController(AppDbContext db) : Controller
{
    public const int PageSize = 200;

    public async Task<IActionResult> Index(string? kind, string? q, CancellationToken ct)
    {
        var leads = db.Leads.AsNoTracking();
        if (Enum.TryParse<LeadKind>(kind, ignoreCase: true, out var k)) leads = leads.Where(l => l.Kind == k);
        if (string.Equals(kind, "paid", StringComparison.OrdinalIgnoreCase)) leads = leads.Where(l => l.Status == LeadStatus.PaymentReported);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = $"%{q.Trim()}%";
            leads = leads.Where(l => EF.Functions.ILike(l.Name, term) || EF.Functions.ILike(l.Mobile, term)
                || (l.Email != null && EF.Functions.ILike(l.Email, term)) || (l.BookingRef != null && EF.Functions.ILike(l.BookingRef, term)));
        }

        ViewData["Kind"] = kind;
        ViewData["Query"] = q;
        ViewData["Total"] = await db.Leads.CountAsync(ct);
        return View(await leads.OrderByDescending(l => l.CreatedAt).Take(PageSize).ToListAsync(ct));
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var lead = await db.Leads.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lead is null) return NotFound();
        ViewData["Submissions"] = await db.LeadSubmissions.AsNoTracking()
            .Where(s => s.LeadId == id).OrderBy(s => s.ReceivedAt).ToListAsync(ct);
        return View(lead);
    }
}
