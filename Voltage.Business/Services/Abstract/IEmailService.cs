using Voltage.Entities.Models.HelperModels;

namespace Voltage.Business.Services.Abstract;

public interface IEmailService
{
    void SendEmail(E_Message message);
}
