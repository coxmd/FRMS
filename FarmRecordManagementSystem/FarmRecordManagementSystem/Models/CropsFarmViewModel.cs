namespace FarmRecordManagementSystem.Models
{
    public class CropFarmViewModel
    {
        public Farms Farms { get; set; }
        public Crops Crops { get; set; }
        public List<FarmPartitions> Partitions { get; set; }

        public string? Name { get; set; }
        public string? Variety { get; set; }
        public int? FarmId { get; set; }
        public decimal FarmSizePlanted { get; set; }
        public decimal QuantityPlanted { get; set; }
        public decimal PartitionPlanted { get; set; }

        public DateTime PlantingDate { get; set; }
    }
}