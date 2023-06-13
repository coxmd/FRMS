using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class AppUsers
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}