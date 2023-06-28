using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class AppUsers
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}