namespace BpstAcademy.Web.Options;

/// <summary>Public site settings, bound from the "Site" section of appsettings.json.</summary>
public sealed class SiteOptions
{
    public const string SectionName = "Site";

    /// <summary>Public address used for canonical, Open Graph and JSON-LD URLs. No trailing slash.</summary>
    public string BaseUrl { get; set; } = "https://edu.bitprosofttech.com";

    /// <summary>End of the ₹49 pre-booking offer (offer strip countdown).</summary>
    public DateTimeOffset OfferDeadline { get; set; }

    public string Absolute(string path) => BaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
}
