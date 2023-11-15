namespace FarmRecordManagementSystem.Models
{
    public class Crops
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Variety { get; set; }
        public string Status { get; set; }
        public int? FarmId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal FarmSizePlanted { get; set; }
        public decimal QuantityPlanted { get; set; }
        public decimal PartitonPlanted { get; set; }
        public decimal ExpectedHarvestedQuantity { get; set; }
        public int ExpectedBagsHarvested { get; set; }
        public DateTime ExpectedHarvestDate { get; set; }
        public DateTime PlantingDate { get; set; }
    }
}