namespace FarmRecordManagementSystem.Models
{
    public class Crops
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Variety { get; set; }
        public int? FarmId { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public DateTime? PlantingDate { get; set; }
    }
}