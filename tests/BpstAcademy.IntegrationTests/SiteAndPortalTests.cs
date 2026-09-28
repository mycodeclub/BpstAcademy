using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BpstAcademy.IntegrationTests;

[Collection(AppCollection.Name)]
public class SiteAndPortalTests(AppFactory factory)
{
    private HttpClient Client() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Homepage_is_indexable_with_canonical_and_structured_data()
    {
        var html = await Client().GetStringAsync("/", TestContext.Current.CancellationToken);

        Assert.Contains("<link rel=\"canonical\" href=\"https://edu.bitprosofttech.com/\" />", html);
        Assert.Contains("<meta name=\"robots\" content=\"index, follow", html);
        Assert.Contains("\"@context\": \"https://schema.org\"", html);
        Assert.Contains("₹49", html);
    }

    [Fact]
    public async Task Unknown_page_returns_404_with_friendly_page()
    {
        var response = await Client().GetAsync("/no-such-page", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Page Not Found", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("/about")]
    [InlineData("/courses")]
    [InlineData("/prebook")]
    [InlineData("/contact")]
    [InlineData("/verify")]
    [InlineData("/courses/java-full-stack")]
    [InlineData("/courses/soc-analyst")]
    public async Task Site_pages_render_with_their_own_canonical(string path)
    {
        var html = await Client().GetStringAsync(path, TestContext.Current.CancellationToken);

        Assert.Contains($"<link rel=\"canonical\" href=\"https://edu.bitprosofttech.com{path}\" />", html);
        Assert.Contains("id=\"main-content\"", html);
    }

    [Theory]
    [InlineData("/courses/no-such-course")]
    [InlineData("/Contact.html")]
    [InlineData("/courses/../appsettings")]
    public async Task Unknown_site_pages_return_404(string path)
    {
        var response = await Client().GetAsync(path, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Every_internal_link_on_the_public_site_resolves()
    {
        var client = Client();
        var ct = TestContext.Current.CancellationToken;
        var seen = new HashSet<string> { "/" };
        var queue = new Queue<string>(seen);
        var broken = new List<string>();

        while (queue.Count > 0)
        {
            var page = queue.Dequeue();
            var html = await client.GetStringAsync(page, ct);
            foreach (System.Text.RegularExpressions.Match m in
                     System.Text.RegularExpressions.Regex.Matches(html, "(?:href|src)=\"(/[^\"#?]*)"))
            {
                var path = m.Groups[1].Value;
                if (path.StartsWith("/portal", StringComparison.Ordinal) || !seen.Add(path)) continue;

                var response = await client.GetAsync(path, ct);
                if (response.StatusCode != HttpStatusCode.OK) broken.Add($"{path} ({(int)response.StatusCode}, linked from {page})");
                else if (response.Content.Headers.ContentType?.MediaType == "text/html") queue.Enqueue(path);
            }
        }

        Assert.True(seen.Count > 100, $"Crawled only {seen.Count} URLs");
        Assert.Empty(broken);
    }

    [Theory]
    [InlineData("/portal/me/dashboard")]
    [InlineData("/portal/me")]
    public async Task Portal_pages_redirect_anonymous_users_to_login(string url)
    {
        var response = await Client().GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("/portal/account/login", response.Headers.Location!.PathAndQuery);
    }

    [Fact]
    public async Task Login_page_is_open_and_not_indexed()
    {
        var html = await Client().GetStringAsync("/portal/account/login", TestContext.Current.CancellationToken);

        Assert.Contains("noindex, nofollow", html);
    }

    [Fact]
    public async Task Health_check_reports_database_ok()
    {
        var response = await Client().GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
