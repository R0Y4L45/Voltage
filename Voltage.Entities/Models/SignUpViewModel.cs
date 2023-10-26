using System.ComponentModel.DataAnnotations;

namespace Voltage.Core.Models;

public class SignUpViewModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Photo { get; set; }
    
    [Required(ErrorMessage = "Please choice birth of date")]
    public DateTime DateOfBirth { get; set; }
}
