using System.ComponentModel.DataAnnotations;

namespace Voltage.Entities.Models.ViewModels;

public class LogInViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
    public string ?UserName {  get; set; }
}
