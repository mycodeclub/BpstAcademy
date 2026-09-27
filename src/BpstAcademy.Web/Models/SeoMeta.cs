namespace BpstAcademy.Web.Models;

/// <summary>
/// Per-page SEO data for the public layout. Set it in a view with <c>ViewData["Seo"] = new SeoMeta { ... }</c>.
/// Paths are site-relative ("/courses"); the layout makes them absolute with <see cref="Options.SiteOptions.BaseUrl"/>.
/// </summary>
public sealed class SeoMeta
{
    public const string IndexRobots = "index, follow, max-image-preview:large, max-snippet:-1, max-video-preview:-1";
    public const string NoIndexRobots = "noindex, nofollow";

    public required string Title { get; init; }
    public required string Description { get; init; }

    /// <summary>Canonical path. Null means the page has no canonical link (error pages).</summary>
    public string? CanonicalPath { get; init; }
    public string Robots { get; init; } = IndexRobots;

    public string? OgTitle { get; init; }
    public string? OgDescription { get; init; }
    public string OgImage { get; init; } = "/site/img/og-bpst-edu.png";
    public string? OgImageAlt { get; init; }
    public string OgType { get; init; } = "website";
}
