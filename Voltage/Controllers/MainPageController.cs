using Microsoft.AspNetCore.Mvc;

namespace Voltage.Controllers
{
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
