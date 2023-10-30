using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Abstract;

public interface ISignUpService
{
    Task<IdentityResult> SignUpAsync(SignUpViewModel model);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<User> GetUserByEmailAsync(string email);
    Task<string> GenerateToken(User user);
}
