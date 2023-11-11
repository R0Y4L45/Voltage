using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract;

public interface ISignInManagerService
{
    Task SignOutAsync();
    Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
    Task<AuthenticationProperties> GetExternalLoginProperties(string provider, string redirectUrl);
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure);
}
