using Microsoft.AspNetCore.Identity;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;
using Voltage.Entities.Models;

namespace Voltage.Business.Services.Concrete
{
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public EmailConfirmationService(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<bool> SendEmailConfirmationAsync(User user, string callbackUrl)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = callbackUrl + $"?token={token}&email={user.Email}";
            var message = new Message(new string[] { user.Email }, "Confirmation Email Link", confirmationLink);

            try
            {
                _emailService.SendEmail(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }


}
