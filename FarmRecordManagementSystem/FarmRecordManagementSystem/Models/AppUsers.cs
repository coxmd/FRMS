using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class AppUsers
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "The Confirm Password field is required.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string? Role { get; set; }
        public string? AccountStatus { get; set; }
    }
}