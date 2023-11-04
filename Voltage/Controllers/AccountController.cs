using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Voltage.Entities.Entity;
using Voltage.Core.Models;
using Voltage.Entities.Models.ViewModels;
using Voltage.Entities.Models;
using Voltage.Business.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Claims;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private readonly ISignUpService _signUpService;
    private readonly ILogInService _logInService;
    private readonly IEmailService _emailService;
    private readonly IUserModifierService _userModifierService;

    public AccountController(ISignUpService signUpService, ILogInService logInService, IEmailService emailService, IUserModifierService userModifierService)
    {
        _signUpService = signUpService;
        _logInService = logInService;
        _emailService = emailService;
        _userModifierService = userModifierService;
    }

    [HttpGet]
    public IActionResult Login() => View();
    [HttpGet]
    public IActionResult SignUp() => View();
    [HttpGet]
    public IActionResult ForgotPassword() => View();
    [HttpGet]
    public IActionResult TermsPolicy() => View();

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _logInService.SignOutAsync();
        return RedirectToAction("Login", "Account", new { area = "" });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        if (await _signUpService.GetUserByEmailAsync(email) is User user)
        {
            if ((await _signUpService.ConfirmEmailAsync(user, token)).Succeeded)
            {
                SignUpViewModel model = new SignUpViewModel { UserName = user.UserName };
                return View("SuccessPage", model);
            }
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLogin(string returnUrl)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { area = "",  returnUrl });
        return new ChallengeResult("Google", await _logInService.GetExternalLoginProperties(redirectUrl!));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError)
    {
        if (remoteError != null)
        {
            return RedirectToAction("Login");
        }

        var externalLoginInfo = _logInService.GetExternalLoginInfoAsync().Result;

        if (externalLoginInfo == null)
        {
            return RedirectToAction(nameof(Error));
        }

        var result = _logInService.ExternalLoginSignInAsync(externalLoginInfo).Result;
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("index", "MainPage", new { area = "User" });
        }

        var existsUser = await _logInService.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
        if(existsUser != null)
        {
            var signInResult = _logInService.ExternalLoginSignInAsync(externalLoginInfo).Result;
            if(signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("index", "MainPage", new { area = "User" });
            }
        }
        else
        {
            var userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var userName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            var user = new User { UserName = "Niko12_", Email = userEmail };

            var createdResult = _userModifierService.CreateAsync(user).Result;
            if(createdResult.Succeeded)
            {
                var addLoginResult = _logInService.AddLoginAsync(user, externalLoginInfo).Result;
                if(addLoginResult.Succeeded)
                {
                    string? token = await _signUpService.GenerateEmailTokenAsync(await _signUpService.GetUserByEmailAsync(user.Email)),
                    callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = user.Email }, Request.Scheme);

                    Message message = new Message(new string[] { user.Email }, "Confirmation Email Link", callbackUrl!);
                    _emailService.SendEmail(message);

                    SignUpViewModel nvvm = new SignUpViewModel { UserName = user.UserName, Email = user.Email };
                    return View("MailCheck", nvvm);
                }
            }
        }
        return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LogInViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (_logInService.LogInAsync(model).Result!)
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
            IdentityResult result = await _signUpService.SignUpAsync(model);
            if (result.Succeeded)
            {
                string? token = await _signUpService.GenerateEmailTokenAsync(await _signUpService.GetUserByEmailAsync(model.Email)),
                callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = model.Email }, Request.Scheme);

                Message message = new Message(new string[] { model.Email }, "Confirmation Email Link", callbackUrl!);
                _emailService.SendEmail(message);

                SignUpViewModel nvvm = new SignUpViewModel { UserName = model.UserName, Email = model.Email };
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _signUpService.GetUserByEmailAsync(model.Email) is User user)
            {
                string? token = await _signUpService.GenerateResetTokenAsync(user),
                    newlink = Url.Action("ResetPassword", "Account", new { area = "", token, email = user.Email }, Request.Scheme);
                Message message = new Message(new string[] { user.Email }, "Forgot password link", newlink!);

                _emailService.SendEmail(message);
                return View("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError(string.Empty, "User not found");
        }

        return View();
    }

    public IActionResult ResetPassword(string token, string email)
    {
        ResetPasswordViewModel viewModel = new ResetPasswordViewModel
        {
            Token = token,
            Email = email
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_signUpService.GetUserByEmailAsync(model.Email).Result is User user)
            {
                if (!await _signUpService.CheckPasswordAsync(user, model.Password))
                {
                    IdentityResult result = await _signUpService.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                        return View("ResetPasswordConfirmation");

                    result.Errors.ToList().ForEach(_ =>
                    {
                        ModelState.AddModelError("Errors", _.Description);
                    });
                }
                
                ModelState.AddModelError("PasswordErrors", "Password was used");
            }

            ModelState.AddModelError("Errors", "User not found");
        }

        return View(model);
    }

    //Isetesen Error page yaza bilersen ki, user, admin ve ya her hansi bir methoda sehv bir sey gonderilen zaman bu sehife erroru gostersin...
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string message)
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = message });
    }
}