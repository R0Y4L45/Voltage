using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Voltage.Entities.Entity;

namespace Voltage.Helper.Validations;

public class CustomUserValidation : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        List<IdentityError> errors = new List<IdentityError>();

        if (char.IsDigit(user.UserName[0]))
            errors.Add(new IdentityError { Code = "UserNameNumberStartWith", Description = "Username shouldn't begin with digits..." });
        if (!user.UserName.IsContainSpecialChar(new char[] {'.', '_', '-'}))
            errors.Add(new IdentityError { Code = "UserNameMustBeContain", Description = "Username should use '.','_','-' characters" });
        if (char.IsLower(user.UserName, 0))
            errors.Add(new IdentityError { Code = "UserNameToUpper", Description = "Username must start with an uppercase letter" });
        if(!user.UserName.Any(char.IsDigit))
            errors.Add(new IdentityError { Code = "UserNameIsDigit", Description = "Username should be contains digits" });
        if (user.UserName.Length < 3 && user.UserName.Length > 25)
            errors.Add(new IdentityError { Code = "UserNameLength", Description = "Username must contain 3-15 characters..." });
        if (user.Email.Length > 70)
            errors.Add(new IdentityError { Code = "EmailLength", Description = "Email can't be larger than 70..." });


        if (!errors.Any())
            return Task.FromResult(IdentityResult.Success);
        return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
    }

    
}
