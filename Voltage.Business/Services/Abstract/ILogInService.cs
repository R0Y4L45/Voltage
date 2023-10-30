using Voltage.Entities.Models.ViewModels;

namespace Voltage.Business.Services.Abstract;

public interface ILogInService
{
    Task<bool> LogInAsync(LogInViewModel model);
    Task SignOutAsync();
}
