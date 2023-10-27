using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract
{
    public interface IEmailConfirmationService
    {
        Task<bool> SendEmailConfirmationAsync(User user, string callbackUrl);
    }
}
