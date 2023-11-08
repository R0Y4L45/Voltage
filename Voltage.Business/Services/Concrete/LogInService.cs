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

    public async Task<(bool IsLocked, TimeSpan? RemainingLockoutTime)> LogInAsync(LogInViewModel model)
    {
        await _signInManager.SignOutAsync();
        User user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                return (false, null);
            }
            else
            {
                int failcount = await _userManager.GetAccessFailedCountAsync(user);
                if (result.IsLockedOut)
                {
                    var remainingLockoutTime = await _userManager.GetLockoutEndDateAsync(user);
                    if (remainingLockoutTime.HasValue)
                    {
                        var timeRemaining = remainingLockoutTime.Value - DateTimeOffset.UtcNow;
                        return (true, timeRemaining);
                    }
                    else
                    {
                        return (true, null);
                    }
                }
                else
                {
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