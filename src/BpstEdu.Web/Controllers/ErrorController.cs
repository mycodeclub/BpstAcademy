using Microsoft.AspNetCore.Mvc;

namespace BpstEdu.Web.Controllers;

/// <summary>Friendly error pages for the public site (re-executed by UseStatusCodePages / UseExceptionHandler).</summary>
[ApiExplorerSettings(IgnoreApi = true)]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorController : Controller
{
    [Route("error/{code:int}")]
    public IActionResult Status(int code)
    {
        Response.StatusCode = code;
        return code == StatusCodes.Status404NotFound ? View("NotFound") : View("Error", code);
    }
}
