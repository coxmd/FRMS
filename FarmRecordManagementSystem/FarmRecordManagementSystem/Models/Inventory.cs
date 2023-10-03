namespace FarmRecordManagementSystem.Models
{
    public class Inventory
    {
        public int? Id { get; set; }

        public string? CropName { get; set; }
        public int? QuantityHarvested { get; set; }
        public int? TotalSold { get; set; }
        public int? PriceSold { get; set; }
        public int? QuantityRemaining { get; set; }
        public int? FarmId { get; set; }
        public int? Sales { get; set; }
        public int? StorageLocation { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}