using Microsoft.AspNetCore.Identity;
using Voltage.Entities.Entity;

namespace Voltage.Helper.Validations;

public class CustomIdentityValidation : IPasswordValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
    {
        List<IdentityError> errors = new List<IdentityError>();
        if (password.ToLower().Contains(user.UserName.ToLower())) 
            errors.Add(new IdentityError { Code = "PasswordContainsUserName", Description = "Please don't use username in the password" });

        if (!errors.Any())
            return Task.FromResult(IdentityResult.Success);
        else
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
    }
}
