using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Voltage.Entities.Models.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Compare("Password",ErrorMessage = "The Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please choice birth of date")]
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
