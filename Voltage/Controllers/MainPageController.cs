using Microsoft.AspNetCore.Mvc;

namespace Voltage.Controllers
{
    //Bunu user-lerin sehifesine atacam yeqin ki, sen ne dusunursen??
    public class MainPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
