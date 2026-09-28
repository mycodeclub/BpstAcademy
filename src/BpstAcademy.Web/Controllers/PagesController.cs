using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace BpstAcademy.Web.Controllers;

/// <summary>
/// Public website pages ported from NewLayout/Landing Page UI: <c>/{slug}</c> renders Views/Pages/{slug}.cshtml and
/// <c>/courses/{slug}</c> renders Views/Courses/{slug}.cshtml. Unknown slugs get the friendly 404 page.
/// Course pages move to the database in B2; the URLs stay the same.
/// </summary>
public class PagesController(ICompositeViewEngine viewEngine) : Controller
{
    [HttpGet("{slug:regex(^[[a-z0-9-]]+$)}")]
    public IActionResult Page(string slug) => Render($"~/Views/Pages/{slug}.cshtml");

    [HttpGet("courses/{slug:regex(^[[a-z0-9-]]+$)}")]
    public IActionResult Course(string slug) => Render($"~/Views/Courses/{slug}.cshtml");

    private IActionResult Render(string viewPath) =>
        viewEngine.GetView(executingFilePath: null, viewPath, isMainPage: true).Success ? View(viewPath) : NotFound();
}
