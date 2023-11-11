using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Concrete;

public class SignInManagerService : ISignInManagerService
{
    private readonly SignInManager<User> _signInManager;

    public SignInManagerService(SignInManager<User> signInManager) =>
        _signInManager = signInManager;

    public async Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info) =>
        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync() =>
        await _signInManager.GetExternalLoginInfoAsync();

    public async Task<AuthenticationProperties> GetExternalLoginProperties(string provider, string redirectUrl) =>
        await Task.FromResult(_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl));

    public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberMe, bool lockoutOnFailure) => 
        await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure);

    public async Task SignOutAsync() => 
        await _signInManager.SignOutAsync();
}
