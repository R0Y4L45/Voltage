using Voltage.Entities.Models.ViewModels;

namespace Voltage.Services.Abstract;

public interface ILogInService
{
    Task<(bool IsLocked, TimeSpan? RemainingLockoutTime)> LogInAsync(LogInViewModel model);
}
