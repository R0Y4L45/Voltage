using Voltage.Business.Services.Abstract;

namespace Voltage.Business.CustomHelpers
{
    public class GenerateUserName
    {
        private readonly IUserModifierService _userModifierService;

        public GenerateUserName(IUserModifierService userModifierService)
        {
            _userModifierService = userModifierService;
        }

        public async Task<string> GenerateRandomUsername(string originalName, int minLength = 8)
        {
            var random = new Random();
            var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0";
            var allowedNumbers = "123456789";
            var specialChars = "-._";

            var baseName = originalName.Split()[0];
            var username = baseName;


            username += specialChars[random.Next(0, specialChars.Length)];
            while (username.Length < minLength)
            {
                username += allowedChars[random.Next(0, allowedChars.Length)];
                username += allowedNumbers[random.Next(0, allowedNumbers.Length)];
            }
            while (await _userModifierService.IsUsernameExistsAsync(username))
            {
                username = baseName; 
                while (username.Length < minLength)
                {
                    username += allowedChars[random.Next(0, allowedChars.Length)];
                }
            }

            return username;
        }

    }
}
