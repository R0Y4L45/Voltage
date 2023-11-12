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
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
