using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Voltage.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
