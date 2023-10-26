using App.Business.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Business.Services;
using Voltage.Entities.Entity;
using Voltage.Models;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public IActionResult Login()
    {
        return View(new LogInViewModel());
    }

    [HttpPost]
    public IActionResult Login(LogInViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if ((bool)new LogInService(_signInManager, _userManager)?.LogIn(model).Result!)
                    return View();
            }
            catch (Exception ex)
            {
                
            }
            return View(model);
        }
        return View();
    }


    public IActionResult Register()
    {
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}