namespace FarmRecordManagementSystem.Models
{
    public class Expenses
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? Category { get; set; }

        public int? FarmId { get; set; }
        public string CategoryName { get; set; } // Add this property for category name
        public int? PartitionId { get; set; }
        public int? CropId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}