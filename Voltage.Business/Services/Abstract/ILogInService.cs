using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Abstract;

public interface ILogInService
{
    Task<(bool IsLocked, TimeSpan? RemainingLockoutTime)> LogInAsync(LogInViewModel model);
    Task SignOutAsync();
    Task<AuthenticationProperties> GetExternalLoginProperties(string redirectUrl);
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
    Task<User> FindByLoginAsync(string loginProvider, string providerKey);
    Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info);
}
