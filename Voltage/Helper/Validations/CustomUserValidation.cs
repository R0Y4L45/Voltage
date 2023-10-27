using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;

namespace Voltage.Helper.Validations;

public class CustomUserValidation : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        List<IdentityError> errors = new List<IdentityError>();

        if (int.TryParse(user.UserName[0].ToString(), out int _))
            errors.Add(new IdentityError { Code = "UserNameNumberStartWith", Description = "Username shouldn't begin with digits..." });
        if (user.UserName.Length < 3 && user.UserName.Length > 25) 
            errors.Add(new IdentityError { Code = "UserNameLength", Description = "Username must contain 3-15 characters..." });
        if (user.Email.Length > 70)
            errors.Add(new IdentityError { Code = "EmailLenhth", Description = "Email can't be bigger than 70..." });

        if (!errors.Any())
            return Task.FromResult(IdentityResult.Success);
        return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
    }
}
