using Voltage.Business.Services.Abstract;

namespace Voltage.Business.CustomHelpers;

public class GenerateUserName
{
    private readonly IUserManagerService _userManagerService;

    public GenerateUserName(IUserManagerService userManagerService)
    {
        _userManagerService = userManagerService;
    }

    public async Task<string> GenerateRandomUsername(string originalName, int minLength = 8)
    {
        var random = new Random();
        string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0",
            allowedNumbers = "123456789",
            specialChars = "-._",
            baseName = originalName.Split()[0],
            username = baseName;

        username = char.IsDigit(username[0]) ? allowedChars[random.Next(0, allowedChars.Length)] + username.Substring(1) : username;
        username += specialChars[random.Next(0, specialChars.Length)];
        while (username.Length < minLength)
        {
            username += allowedChars[random.Next(0, allowedChars.Length)];
            username += allowedNumbers[random.Next(0, allowedNumbers.Length)];
        }
        while (await _userManagerService.FindByNameAsync(username) is not null)
        {
            username = baseName; 
            while (username.Length < minLength)
                username += allowedChars[random.Next(0, allowedChars.Length)];
        }

        return username;
    }

}
