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
        public string? SoilType { get; set; }
    }
}