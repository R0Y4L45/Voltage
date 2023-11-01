using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Concrete;

public class LogInService : ILogInService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LogInService(SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ExternalLoginViewModel> GetExternalLoginProperties(string provider, string redirectUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        
        var providerDisplayName = provider;

        var externalLoginViewModel = new ExternalLoginViewModel
        {
            Provider = providerDisplayName,
            RedirectUrl = properties.RedirectUri
        };

        return externalLoginViewModel;
    }

    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(HttpContext context)
    {
        return await _signInManager.GetExternalLoginInfoAsync();
    }

    public async Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info)

    {
        return await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
    }

    public async Task<bool> LogInAsync(LogInViewModel model)
    {
        await _signInManager.SignOutAsync();
        User user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, true);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return await Task.FromResult(true);
            }
            else
            {
                await _userManager.AccessFailedAsync(user);

                int failcount = await _userManager.GetAccessFailedCountAsync(user);
                if (failcount == 3)
                {
                    await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(1)));
                    throw new Exception("After 3 unsuccessful attempts your account is locked for 1 minute.");
                }
                else
                {
                    if (result.IsLockedOut)
                        throw new Exception("After 3 unsuccessful attempts your account is locked for 1 minute.");
                    else
                        throw new Exception("Password is wrong. Check and try again."); 
                }
            }
        }

        throw new Exception("Such user not exist");
    }

    public async Task SignOutAsync() => await _signInManager.SignOutAsync();
}