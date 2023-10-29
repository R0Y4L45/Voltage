using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Concrete;

public class LogInService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    public LogInService(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<bool> LogIn(LogInViewModel model)
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
}
