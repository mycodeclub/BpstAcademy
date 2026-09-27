using Microsoft.AspNetCore.Mvc;

namespace BpstAcademy.Web.Areas.Me.Controllers;

/// <summary>Self-service home for every signed-in user. Widgets are shown by permission from B1 onwards.</summary>
[Area("Me")]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}
