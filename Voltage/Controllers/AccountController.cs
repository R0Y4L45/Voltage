using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;
using Voltage.Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Voltage.Business.CustomHelpers;
using Voltage.Entities.Models.HelperModels;
using Voltage.Services.Abstract;

namespace Voltage.Controllers;

public class AccountController : Controller
{
    private readonly ISignUpService _signUpService;
    private readonly ILogInService _logInService;
    private readonly IEmailService _emailService;
    private readonly IUserManagerService _userManagerService;
    private readonly ISignInManagerService _signInManagerService;

    public AccountController(ISignUpService signUpService, ILogInService logInService, IEmailService emailService, IUserManagerService userManagerService, ISignInManagerService signInManagerService)
    {
        _signUpService = signUpService;
        _logInService = logInService;
        _emailService = emailService;
        _userManagerService = userManagerService;
        _signInManagerService = signInManagerService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LogInViewModel model)
    {
        if (ModelState.IsValid)
            try
            {
                var (isLocked, remainingLockoutTime) = await _logInService.LogInAsync(model);

                if (isLocked)
                    ViewBag.RemainingLockoutTime = remainingLockoutTime;
                else
                    return RedirectToAction("Index", "MainPage", new { area = "User" });

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

        return View();
    }

    [HttpGet]
    public IActionResult SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        try
        {
            if (model.Agree)
            {
                IdentityResult result = await _signUpService.SignUpAsync(model);
                if (result.Succeeded)
                {
                    string? token = await _userManagerService.GenerateEmailTokenAsync(await _userManagerService.FindByEmailAsync(model.Email)),
                    callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = model.Email }, Request.Scheme);

                    E_Message message = new E_Message(new string[] { model.Email }, "Confirmation Email Link", callbackUrl!);
                    _emailService.SendEmail(message);

                    SignUpViewModel nvvm = new SignUpViewModel { UserName = model.UserName, Email = model.Email };

                    return View("MailCheck", nvvm);
                }
                result.Errors.ToList().ForEach(_ => ModelState.AddModelError(_.Code, _.Description));
            }
            else ModelState.AddModelError("Agree", "You must agree to the terms and policy to register.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Error", ex.Message);
        }

        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpGet]
    public IActionResult TermsPolicy() => View();

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManagerService.SignOutAsync();
        return RedirectToAction("Login", "Account", new { area = "" });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        if (await _userManagerService.FindByEmailAsync(email) is User user)
            if ((await _userManagerService.ConfirmEmailAsync(user, token)).Succeeded)
                return View("SuccessPage", new SignUpViewModel { UserName = user.UserName });

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLogin(string returnUrl)
    {
        string? redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { area = "", returnUrl });
        return new ChallengeResult("Google", await _signInManagerService.GetExternalLoginProperties("Google", redirectUrl!));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl, string remoteError)
    {
        if (remoteError != null) return RedirectToAction("Login");

        if (await _signInManagerService.GetExternalLoginInfoAsync() is not ExternalLoginInfo externalLoginInfo) return NotFound();

        if ((await _signInManagerService.ExternalLoginSignInAsync(externalLoginInfo)).Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction("index", "MainPage", new { area = "User" });
        }

        if (await _userManagerService.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey) is User)
        {
            if ((await _signInManagerService.ExternalLoginSignInAsync(externalLoginInfo)).Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                else return RedirectToAction("index", "MainPage", new { area = "User" });
            }
        }
        else
        {
            GenerateUserName gun = new(_userManagerService);
            string userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                generatedUsername = gun.GenerateUserNameByEmail(userEmail);

            User user = new() { UserName = generatedUsername, Email = userEmail };

            if ((await _userManagerService.CreateAsync(user)).Succeeded)
            {
                if ((await _userManagerService.AddLoginAsync(user, externalLoginInfo)).Succeeded)
                {
                    await _userManagerService.AddToRoleAsync(user, "User");
                    string? token = await _userManagerService.GenerateEmailTokenAsync(await _userManagerService.FindByEmailAsync(user.Email)),
                    callbackUrl = Url.Action("ConfirmEmail", "Account", new { area = "", token, email = user.Email }, Request.Scheme);

                    _emailService.SendEmail(new E_Message(new string[] { user.Email }, "Confirmation Email Link", callbackUrl!));

                    return View("MailCheck", new SignUpViewModel { UserName = user.UserName, Email = user.Email });
                }
            }
        }
        return RedirectToAction("Login", new { area = "" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _userManagerService.FindByEmailAsync(model.Email) is User user)
            {
                string? newlink = Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        area = "",
                        token = await _userManagerService.GenerateResetTokenAsync(user),
                        email = user.Email
                    },
                    Request.Scheme);

                _emailService.SendEmail(new E_Message(new string[] { user.Email }, "Forgot password link", newlink!));
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
            if (_userManagerService.FindByEmailAsync(model.Email).Result is User user)
            {
                if (!await _userManagerService.CheckPasswordAsync(user, model.Password))
                {
                    IdentityResult result = await _userManagerService.ResetPasswordAsync(user, model.Token, model.Password);

                    if (result.Succeeded)
                        return View("ResetPasswordConfirmation");

                    result.Errors.ToList().ForEach(_ => ModelState.AddModelError("Errors", _.Description));
                }

                ModelState.AddModelError("PasswordErrors", "Password was used");
            }

            ModelState.AddModelError("Errors", "User not found");
        }

        return View(model);
    }


    #region errors
    public IActionResult NotFoundView() => View("NotFoundView");

    public IActionResult ServerError() => View("ServerError");
    #endregion

}