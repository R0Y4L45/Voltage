using Microsoft.AspNetCore.Identity;

namespace Voltage.Helper.Validations;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName) => new IdentityError { Code = "DuplicateUserName", Description = $"\"{userName}\" was used. Please try another one." };
    public override IdentityError InvalidUserName(string userName) => new IdentityError { Code = "InvalidUserName", Description = "Invalid username." };
    public override IdentityError DuplicateEmail(string email) => new IdentityError { Code = "DuplicateEmail", Description = $"\"{email}\" was used other user." };
    public override IdentityError InvalidEmail(string email) => new IdentityError { Code = "InvalidEmail", Description = "Invalid email." };
}
