namespace FarmRecordManagementSystem.Models
{
    public class Crops
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Variety { get; set; }
        public int? FarmId { get; set; }
        public decimal FarmSizePlanted { get; set; }
        public decimal QuantityPlanted { get; set; }
        public decimal ExpectedHarvestedQuantity { get; set; }
        public decimal ExpectedBagsHarvested { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public DateTime? PlantingDate { get; set; }
    }
}