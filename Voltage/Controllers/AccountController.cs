using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Entities.Entity;
using Voltage.Core.Models;
using Voltage.Business.Services.Concrete;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private RoleManager<IdentityRole> _roleManager;
    public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LogInViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if ((bool)new LogInService(_signInManager, _userManager)?.LogIn(model).Result!)
                    return RedirectToAction("index", "VoltageUser", new { area = "User" });
            }
            catch (Exception ex)
            {
                //bu errror sehifeye yonlendirmemelidir. Bunu ele-bele qoymusam burada sene men message gonderecem sen onu login terefde gostereceksen...)
                return RedirectToAction("error", new { area = "", message = ex.Message });
            }
        }
        return View();
    }

    public IActionResult SignUp() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SignUp(SignUpViewModel model)
    {
        try
        {
            IdentityResult result = new SignUpService(_userManager, _roleManager).SignUp(model).Result;

            if (result.Succeeded)
                return RedirectToAction("login", new { area = "" });

            result.Errors.ToList().ForEach(_ => ModelState.AddModelError(_.Code, _.Description));
            return View();
        }
        catch (Exception)
        {
            //yuxarida oldugu kimi message sene gonderilir sen ekranaa verirsen...
            return RedirectToAction("error", new { area = "" });
        }
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(string email)
    {
        return View();
    }

    //Isetesen Error page yaza bilersen ki, user, admin ve ya her hansi bir methoda sehv bir sey gonderilen zaman bu sehife erroru gostersin...
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
    }
}