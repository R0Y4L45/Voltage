using Microsoft.AspNetCore.Identity;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Concrete;

public class SignUpService : ISignUpService
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public SignUpService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> SignUpAsync(SignUpViewModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) == null)
        {
            if (await _userManager.FindByNameAsync(model.UserName) == null)
            {
                User user = new User
                {
                    UserName = model.UserName.Trim(),
                    Email = model.Email.Trim(),
                    DateOfBirth = model.DateOfBirth,
                    Photo = (model.Photo != null) ? await UploadFileHelper.UploadFile(model.Photo) : ""
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        IdentityRole role = new IdentityRole
                        {
                            Name = "User"
                        };

                        if (!(await _roleManager.CreateAsync(role)).Succeeded)
                            throw new Exception("Failed to add role");
                    }

                    return await _userManager.AddToRoleAsync(user, "User");

                }

                return result;
            }

            throw new Exception("This username was used");
        }

        throw new Exception("This email was used");
    }

    public async Task<User> GetUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);
    public async Task<string> GenerateToken(User user) => await _userManager.GeneratePasswordResetTokenAsync(user);
    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) => await _userManager.ConfirmEmailAsync(user, token);
    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password) => await _userManager.ResetPasswordAsync(user, token, password);
}
