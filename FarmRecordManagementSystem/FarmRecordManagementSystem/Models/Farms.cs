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

        [Display(Name = "Number of Partitions")]
        public int? NumberOfPartitions { get; set; }

        public decimal? PartitionSizes { get; set; }
        public string? SoilType { get; set; }

        public int? TotalFarmRevenue { get; set; }
    }
}