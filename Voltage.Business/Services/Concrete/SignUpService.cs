using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Concrete;

public class SignUpService
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public SignUpService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> SignUp(SignUpViewModel vm)
    {
        if (await _userManager.FindByEmailAsync(vm.Email) == null)
        {
            if (await _userManager.FindByNameAsync(vm.UserName) == null)
            {
                string path = (vm.Photo != null) ? await UploadFileHelper.UploadFile(vm.Photo) : "";
                User user = new User
                {
                    UserName = vm.UserName,
                    Email = vm.Email,
                    DateOfBirth = vm.DateOfBirth,
                    Photo = path
                };

                IdentityResult result = await _userManager.CreateAsync(user, vm.Password);
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

}
