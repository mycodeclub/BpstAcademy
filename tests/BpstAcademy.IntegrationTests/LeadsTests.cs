using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using BpstAcademy.Domain.Crm;
using BpstAcademy.Infrastructure.Persistence;
using BpstAcademy.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BpstAcademy.IntegrationTests;

/// <summary>Website forms are saved (pre-bookings before payment) and shown in the portal.</summary>
[Collection(AppCollection.Name)]
public partial class LeadsTests(AppFactory factory) : IAsyncLifetime
{
    private const string Email = "leads-admin@bpst.test";
    private const string Password = "Leads-Test-Pass1";

    public async ValueTask InitializeAsync()
    {
        using var scope = factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IdentitySeeder>().SeedAsync(Email, Password);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Contact_enquiry_is_saved_with_the_raw_submission()
    {
        var response = await Client().PostAsJsonAsync("/api/leads", new
        {
            form_type = "enquiry", name = "Enquiry Test", mobile = "+919876500001", course = "java-full-stack",
            message = "Weekend batch?", consent = true, page = "/contact",
        }, Ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        await using var db = Db();
        var lead = await db.Leads.SingleAsync(l => l.Mobile == "+919876500001", Ct);
        Assert.Equal(LeadKind.Enquiry, lead.Kind);
        Assert.Equal("Weekend batch?", lead.Message);
        Assert.Equal("/contact", lead.SourcePage);
        var submission = await db.LeadSubmissions.SingleAsync(s => s.LeadId == lead.Id, Ct);
        Assert.Equal("enquiry", submission.FormType);
        Assert.Contains("Weekend batch?", submission.Payload);
    }

    [Fact]
    public async Task Prebooking_is_saved_before_payment_and_the_reported_payment_updates_it()
    {
        var client = Client();
        var booking = new
        {
            booking_ref = "BPST-D26-TEST01", name = "Prebook Test", mobile = "9876500002", email = "prebook@bpst.test",
            city = "Lucknow", course = "soc-analyst", course_title = "SOC Analyst", duration = "3-Month Program",
            mode = "Classroom", qualification = "BCA", consent = true, amount = 49,
        };

        var started = await client.PostAsJsonAsync("/api/leads", new Dictionary<string, object>(Props(booking)) { ["form_type"] = "prebook_started" }, Ct);
        Assert.Equal(HttpStatusCode.Created, started.StatusCode);
        await using (var db = Db())
        {
            var lead = await db.Leads.SingleAsync(l => l.BookingRef == "BPST-D26-TEST01", Ct);
            Assert.Equal(LeadStatus.New, lead.Status);
            Assert.Equal("+919876500002", lead.Mobile);
            Assert.Equal(49m, lead.Amount);
        }

        var paid = await client.PostAsJsonAsync("/api/leads", new Dictionary<string, object>(Props(booking))
        {
            ["form_type"] = "prebook_paid", ["payment_method"] = "upi", ["payment_id"] = "UTR123456789",
        }, Ct);
        Assert.Equal(HttpStatusCode.Created, paid.StatusCode);
        await using (var db = Db())
        {
            var lead = await db.Leads.SingleAsync(l => l.BookingRef == "BPST-D26-TEST01", Ct);
            Assert.Equal(LeadStatus.PaymentReported, lead.Status);
            Assert.Equal("UTR123456789", lead.PaymentReference);
            Assert.Equal(2, await db.LeadSubmissions.CountAsync(s => s.LeadId == lead.Id, Ct));
        }
    }

    [Fact]
    public async Task Invalid_mobile_is_rejected_and_nothing_is_saved()
    {
        var response = await Client().PostAsJsonAsync("/api/leads", new { form_type = "enquiry", name = "Bad Mobile", mobile = "12345" }, Ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await using var db = Db();
        Assert.False(await db.Leads.AnyAsync(l => l.Name == "Bad Mobile", Ct));
    }

    [Fact]
    public async Task Honeypot_posts_look_accepted_but_are_not_saved()
    {
        var response = await Client().PostAsJsonAsync("/api/leads", new
        {
            form_type = "enquiry", name = "Spam Bot", mobile = "9876500003", website = "http://spam.example",
        }, Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var db = Db();
        Assert.False(await db.Leads.AnyAsync(l => l.Name == "Spam Bot", Ct));
    }

    [Fact]
    public async Task Enquiries_page_needs_sign_in()
    {
        var response = await Client().GetAsync("/portal/crm/leads", Ct);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/portal/account/login", response.Headers.Location!.PathAndQuery);
    }

    [Fact]
    public async Task Admin_sees_saved_enquiries_in_the_portal()
    {
        var client = Client();
        await client.PostAsJsonAsync("/api/leads", new { form_type = "enquiry", name = "Portal Visible", mobile = "9876500004" }, Ct);
        await SignIn(client);

        var list = await client.GetStringAsync("/portal/crm/leads", Ct);
        Assert.Contains("Portal Visible", list);
        Assert.Contains("Enquiries", list);

        await using var db = Db();
        var id = (await db.Leads.SingleAsync(l => l.Name == "Portal Visible", Ct)).Id;
        var details = await client.GetStringAsync($"/portal/crm/leads/details/{id}", Ct);
        Assert.Contains("9876500004", details); // Razor encodes "+" as &#x2B;
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static IEnumerable<KeyValuePair<string, object>> Props(object o) =>
        o.GetType().GetProperties().Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(o)!));

    private AppDbContext Db() => factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    // The auth cookie is Secure, so the client talks https.
    private HttpClient Client() => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        BaseAddress = new Uri("https://localhost"),
    });

    private static async Task SignIn(HttpClient client)
    {
        var page = await client.GetStringAsync("/portal/account/login", Ct);
        var token = AntiforgeryToken().Match(page).Groups[1].Value;
        var response = await client.PostAsync("/portal/account/login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["LoginId"] = Email,
            ["Password"] = Password,
        }), Ct);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryToken();
}
