using System.ComponentModel.DataAnnotations;

namespace Voltage.Entities.Models.ViewModels;

public class ForgotViewModel
{
    [Required]
    public string Email { get; set; } = null!;
}
