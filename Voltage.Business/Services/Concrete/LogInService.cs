using Microsoft.AspNetCore.Authentication;
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

    public async Task<User> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return await _userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    public async Task<AuthenticationProperties> GetExternalLoginProperties(string redirectUrl)
    {
        return await Task.FromResult(_signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl));
    }

    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
    {
        return await _signInManager.GetExternalLoginInfoAsync();
    }

    public async Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info)
    {
        return await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);
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

    public async Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info)
    {
        return await _userManager.AddLoginAsync(user, info);
    }
}