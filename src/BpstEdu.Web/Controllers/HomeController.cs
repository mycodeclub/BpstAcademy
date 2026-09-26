using Microsoft.AspNetCore.Mvc;

namespace BpstEdu.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
