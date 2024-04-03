using System.Text.RegularExpressions;
using Voltage.Business.Services.Abstract;
using Voltage.Entities.Entity;

namespace Voltage.Business.CustomHelpers;

public class GenerateUserName
{
    private readonly IUserManagerService _userManagerService;

    public GenerateUserName(IUserManagerService userManagerService) =>
        _userManagerService = userManagerService;

    private string GenerateRandomUsername(string originalName, int minLength = 8)
    {
        Random random = new Random();
        string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0",
            allowedCharsPattern = @"[^a-zA-Z]",
            allowedNumbers = "123456789",
            specialChars = "-._",
            baseName = originalName.Split()[0],
            username;

        baseName = baseName
            .Replace("Ö", "O")
            .Replace("ö", "o")
            .Replace("Ü", "U")
            .Replace("ü", "u")
            .Replace("İ", "I")
            .Replace("Ç", "C")
            .Replace("ç", "c")
            .Replace("Ğ", "G")
            .Replace("ğ", "g")
            .Replace("Ə", "E")
            .Replace("ə", "e")
            .Replace("Ş", "S")
            .Replace("ş", "s")
            .Replace("ş", "s")
            .Replace("ş", "s")
            .Replace("ı", "i");
        baseName = Regex.Replace(baseName, allowedCharsPattern, "");

        username = baseName;
        username = char.IsDigit(username[0]) ? allowedChars[random.Next(0, allowedChars.Length)] + username.Substring(1) : username;
        username += specialChars[random.Next(0, specialChars.Length)];
        while (username.Length < minLength)
        {
            username += allowedChars[random.Next(0, allowedChars.Length)];
            username += allowedNumbers[random.Next(0, allowedNumbers.Length)];
        }
        while (_userManagerService.FindByNameAsync(username) is not null)
        {
            username = baseName;
            while (username.Length < minLength)
                username += allowedChars[random.Next(0, allowedChars.Length)];
        }

        return username;
    }

    public string GenerateUserNameByEmail(string email)
    {
        string username = email.Split("@")[0];

        if (char.IsUpper(username[0]))
            username = username.Replace(username[0], char.ToLower(username[0]));
        
        if(username.Length > 15)
            username = username[..5];

        if (!username.Any(char.IsDigit) || username.Length < 3)
            username = GenerateRandomUsername(username);

        return username;
    }
}