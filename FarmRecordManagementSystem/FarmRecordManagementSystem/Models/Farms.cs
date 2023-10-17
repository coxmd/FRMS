using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class Farms
    {
        public int? Id { get; set; }

        [Display(Name = "Farm Name")]
        public string? Name { get; set; }
        [Display(Name = "Farm Size (in hectares)")]
        public int? Size { get; set; }

        [Display(Name = "Has Partitions")]
        public bool HasPartitions { get; set; }
        public List<FarmPartitions> Partitions { get; set; }

        public string? SoilType { get; set; }
        public DateTime DateCreated { get; set; }
        public int? CreatedBy { get; set; }
    }
}