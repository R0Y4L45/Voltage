using Microsoft.AspNetCore.Identity;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;
using Voltage.Services.Abstract;

namespace Voltage.Services.Concrete;

public class LogInService : ILogInService
{
    private readonly ISignInManagerService _signInManagerService;
    private readonly IUserManagerService _userManagerService;

    public LogInService(ISignInManagerService signInManagerService, IUserManagerService userManagerService)
    {
        _signInManagerService = signInManagerService;
        _userManagerService = userManagerService;
    }

    #region Methods

    public async Task<(bool IsLocked, TimeSpan? RemainingLockoutTime)> LogInAsync(LogInViewModel model)
    {
        await _signInManagerService.SignOutAsync();
        User user = await _userManagerService.FindByEmailAsync(model.Email);

        if (user != null)
        {
            var result = await _signInManagerService.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                await _userManagerService.ResetAccessFailedCountAsync(user);
                return (false, null);
            }
            else
            {
                if (result.IsLockedOut)
                {
                    DateTimeOffset? remainingLockoutTime = await _userManagerService.GetLockoutEndDateAsync(user);
                    if (remainingLockoutTime.HasValue)
                    {
                        TimeSpan timeRemaining = remainingLockoutTime.Value - DateTimeOffset.UtcNow;
                        return (true, timeRemaining);
                    }
                    else
                        return (true, null);
                }
                else
                    throw new Exception("Password is wrong. Check and try again.");
            }
        }

        throw new Exception("Such user not exist");
    }

    #endregion
}