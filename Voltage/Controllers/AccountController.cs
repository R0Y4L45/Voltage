using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Entities.Entity;
using Voltage.Core.Models;
using Voltage.Business.Services.Concrete;
using Voltage.Entities.Models.ViewModels;
using Voltage.Business.Services.Abstract;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private RoleManager<IdentityRole> _roleManager;
    private readonly SignUpService _signUpService;
    public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, SignUpService signUpService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _signUpService = signUpService;
    }

    public IActionResult Login() => View();
    public IActionResult SignUp() => View();

    public IActionResult SuccessPage() => View();

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
                //return RedirectToAction("error", new { area = "", message = ex.Message });
                ModelState.AddModelError("Error", ex.Message);
            }
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        try
        {
            var callbackUrl = Url.Action("ConfirmEmail", "Account", null, Request.Scheme);
            IdentityResult result = await _signUpService.SignUp(model, callbackUrl!);

            if (result.Succeeded)
            {
                var nvvm = new SignUpViewModel { UserName = model.UserName };
                return RedirectToAction("SuccessPage", nvvm);
            }

            result.Errors.ToList().ForEach(_ => ModelState.AddModelError(_.Code, _.Description));
        }
        catch (Exception ex)
        {
            //yuxarida oldugu kimi message sene gonderilir sen ekranaa verirsen...
            //return RedirectToAction("error", new { area = "" });
            ModelState.AddModelError("Error", ex.Message);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var vm = new SignUpViewModel { UserName = user.UserName };
                return View("SuccessPage", vm);
            }
        }
        return View();
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