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
        if (await _userManager.FindByEmailAsync(model.Email) is User user)
        {
            if (_signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false).Result.Succeeded)
                return await Task.FromResult(true);

            throw new Exception("Password is false. Please, check password and try again");
        }

        throw new Exception("User not found. Please, check email and try again.");
    }
}
