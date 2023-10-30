using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voltage.Entities.Entity;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    //private readonly UserManager<User>? _userManager;

    //public MainPageController(UserManager<User>? userManager)
    //{
    //    _userManager = userManager;

    //}

    public IActionResult Index()
    {

        return View();
    }
}
