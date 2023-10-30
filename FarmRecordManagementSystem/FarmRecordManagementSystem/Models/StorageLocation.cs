namespace FarmRecordManagementSystem.Models
{
    public class StorageLocation
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public int? FarmId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}