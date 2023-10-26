using Voltage.Entities.Models;

namespace Voltage.Business.Services.Abstract
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
