using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Entities.Entity;
using Voltage.Core.Models;
using Voltage.Business.Services.Concrete;
using Voltage.Entities.Models.ViewModels;
using Voltage.Entities.Models;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private readonly SignUpService _signUpService;
    private readonly Business.Services.Abstract.IEmailService _emailService;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, SignUpService signUpService, Business.Services.Abstract.IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _signUpService = signUpService;
        _emailService = emailService;
    }

    public IActionResult Login() => View();
    public IActionResult SignUp() => View();
    public IActionResult ForgotPassword()=>View();
    public IActionResult TermsPolicy() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LogInViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if ((bool)new LogInService(_signInManager, _userManager)?.LogIn(model).Result!)
                    return RedirectToAction("index", "MainPage", new { area = "User" });
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
            IdentityResult result = await _signUpService.SignUp(model);
            if (result.Succeeded)
            {
                User user = await _userManager.FindByNameAsync(model.UserName);
                string? token = await _userManager.GenerateEmailConfirmationTokenAsync(user),
                    callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = user.Email }, Request.Scheme);

                Message message = new Message(new string[] { user.Email }, "Confirmation Email Link", callbackUrl!);
                _emailService.SendEmail(message);

                SignUpViewModel nvvm = new SignUpViewModel { UserName = user.UserName ,Email = user.Email};
                return View("MailCheck", nvvm);
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

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account", new { area = "" });
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