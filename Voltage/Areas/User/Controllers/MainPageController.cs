using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserEntity = Voltage.Entities.Entity.User;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    private readonly UserManager<UserEntity> _userManager;
    public UserEntity? UserEntity { get; set; }

    public MainPageController(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        TempData["ProfilePhoto"] = _userManager.FindByNameAsync(User.Identity?.Name).Result.Photo;

        return View();
    }
}
