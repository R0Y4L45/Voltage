using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Models.ViewModels;

namespace Voltage.Services.Abstract;

public interface ISignUpService
{
    Task<IdentityResult> SignUpAsync(SignUpViewModel model);
}
