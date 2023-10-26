using Microsoft.AspNetCore.Mvc;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
public class VoltageUserController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
    public ActionResult Privacy()
    {
        return View();
    }
}
