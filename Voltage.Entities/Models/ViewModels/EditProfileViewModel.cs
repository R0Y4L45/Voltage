using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Voltage.Entities.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Country { get; set; }
    }
}
