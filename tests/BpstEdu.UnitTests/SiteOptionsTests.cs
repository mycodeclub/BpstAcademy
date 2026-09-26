using BpstEdu.Web.Options;

namespace BpstEdu.UnitTests;

public class SiteOptionsTests
{
    [Theory]
    [InlineData("https://edu.bitprosofttech.com", "/", "https://edu.bitprosofttech.com/")]
    [InlineData("https://edu.bitprosofttech.com/", "/courses", "https://edu.bitprosofttech.com/courses")]
    [InlineData("https://edu.bitprosofttech.com", "site/img/og.png", "https://edu.bitprosofttech.com/site/img/og.png")]
    public void Absolute_joins_base_url_and_path_with_one_slash(string baseUrl, string path, string expected)
    {
        var options = new SiteOptions { BaseUrl = baseUrl };

        Assert.Equal(expected, options.Absolute(path));
    }
}
