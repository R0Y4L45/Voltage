using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voltage.Entities.Entity;

namespace Voltage.Business.Services.Abstract
{
    public interface IEmailConfirmationService
    {
        Task<bool> SendEmailConfirmationAsync(User user, string callbackUrl);
    }
}
