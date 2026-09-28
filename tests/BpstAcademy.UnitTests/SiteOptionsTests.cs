using BpstAcademy.Web.Options;

namespace BpstAcademy.UnitTests;

public class SiteOptionsTests
{
    [Theory]
    [InlineData("https://www.bpstacademy.com", "/", "https://www.bpstacademy.com/")]
    [InlineData("https://www.bpstacademy.com/", "/courses", "https://www.bpstacademy.com/courses")]
    [InlineData("https://www.bpstacademy.com", "site/img/og.png", "https://www.bpstacademy.com/site/img/og.png")]
    public void Absolute_joins_base_url_and_path_with_one_slash(string baseUrl, string path, string expected)
    {
        var options = new SiteOptions { BaseUrl = baseUrl };

        Assert.Equal(expected, options.Absolute(path));
    }
}
