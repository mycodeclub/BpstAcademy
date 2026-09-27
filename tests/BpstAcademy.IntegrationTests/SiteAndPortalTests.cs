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
