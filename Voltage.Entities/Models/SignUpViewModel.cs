namespace Voltage.Models;

public class SignUpViewModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly? DateOfBirth { get; set; } = null!;
    public string? Photo { get; set; }
}
