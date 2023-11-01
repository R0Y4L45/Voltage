using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Voltage.Business.Services.Abstract;
using UserEntity = Voltage.Entities.Entity.User;

namespace Voltage.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = "User, Admin")]
public class MainPageController : Controller
{
    private readonly ISignUpService _signUpService;

    public MainPageController(ISignUpService signUpService)
    {
        _signUpService = signUpService;
    }

    public IActionResult Index()
    {
        TempData["ProfilePhoto"] = _signUpService.GetUserByName(User.Identity?.Name!).Result.Photo;

        return View();
    }

    public IActionResult Message()
    {
        return View();
    }

    [HttpPost]
    public string GetUserId([FromBody] string name = "null")
    {
        if (name != "null")
            return JsonSerializer.Serialize(_signUpService?.GetUserByName(name)?.Result.Id);

        return "null";
    }
}
