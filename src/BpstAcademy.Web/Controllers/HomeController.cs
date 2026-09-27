using Microsoft.AspNetCore.Mvc;

namespace BpstAcademy.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
