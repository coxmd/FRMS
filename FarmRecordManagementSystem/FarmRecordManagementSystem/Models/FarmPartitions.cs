using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class FarmPartitions
    {
        public int? Id { get; set; }

        // [Display(Name = "Farm Name")]
        public string? Name { get; set; }
        // [Display(Name = "Farm Size (in hectares)")]
        public int? Size { get; set; }

        public DateTime DateCreated { get; set; }

        public int FarmId { get; set; }
    }
}