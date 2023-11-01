using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Abstract;

public interface ILogInService
{
    Task<bool> LogInAsync(LogInViewModel model);
    Task SignOutAsync();
    Task<ExternalLoginViewModel> GetExternalLoginProperties(string provider, string redirectUrl);
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync(HttpContext context);
    Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
}
