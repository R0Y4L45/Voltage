using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Voltage.Business.Services.Abstract;
using Voltage.Business.CustomHelpers;
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
                    DateOfBirth = model.DateOfBirth ?? DateTime.Now,
                    Photo = (model.Photo != null) ? await UploadFileHelper.UploadFile(model.Photo) : "https://t4.ftcdn.net/jpg/00/64/67/63/360_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg"
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
    public async Task<User> GetUserByName(string name) => await _userManager.FindByNameAsync(name);
    public async Task<string> GenerateResetTokenAsync(User user) => await _userManager.GeneratePasswordResetTokenAsync(user);
    public async Task<string> GenerateEmailTokenAsync(User user) => await _userManager.GenerateEmailConfirmationTokenAsync(user);
    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) => await _userManager.ConfirmEmailAsync(user, token);
    public async Task<string> GenerateEmailConfirmationTokenAsync(User user) => await _userManager.GenerateEmailConfirmationTokenAsync(user);
    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password) => 
        await _userManager.ResetPasswordAsync(user, token, password);
    public async Task<bool> CheckPasswordAsync(User user, string password) => await _userManager.CheckPasswordAsync(user, password);
    public async Task<List<User>> GetAllUsers(Expression<Func<User, bool>> filter = null!) =>
        filter == null ? await _userManager.Users.ToListAsync() : await _userManager.Users.Where(filter).ToListAsync();

}
