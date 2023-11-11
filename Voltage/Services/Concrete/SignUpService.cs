using Microsoft.AspNetCore.Identity;
using Voltage.Business.CustomHelpers;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;
using Voltage.Services.Abstract;

namespace Voltage.Services.Concrete;

public class SignUpService : ISignUpService
{
    private IUserManagerService _userManagerService;
    private RoleManager<IdentityRole> _roleManager;

    public SignUpService(RoleManager<IdentityRole> roleManager, IUserManagerService userManagerService)
    {
        _roleManager = roleManager;
        _userManagerService = userManagerService;
    }

    public async Task<IdentityResult> SignUpAsync(SignUpViewModel model)
    {
        if (await _userManagerService.FindByEmailAsync(model.Email) == null)
        {
            if (await _userManagerService.FindByNameAsync(model.UserName) == null)
            {
                User user = new User
                {
                    UserName = model.UserName.Trim(),
                    Email = model.Email.Trim(),
                    DateOfBirth = model.DateOfBirth ?? DateTime.Now,
                    Photo = (model.Photo != null) ? await UploadFileHelper.UploadFile(model.Photo) : "https://t4.ftcdn.net/jpg/00/64/67/63/360_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg"
                };

                IdentityResult result = await _userManagerService.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        IdentityResult roleResult = await AddRole("User");
                        if (!roleResult.Succeeded)
                            return roleResult;
                    }

                    return await _userManagerService.AddToRoleAsync(user, "User");
                }

                return result;
            }

            throw new Exception("This username was used");
        }

        throw new Exception("This email was used");
    }
    private async Task<IdentityResult> AddRole(string roleName) =>
        await _roleManager.CreateAsync(new IdentityRole
        {
            Name = roleName
        });
}
