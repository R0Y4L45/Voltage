using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Voltage.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
