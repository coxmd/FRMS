namespace FarmRecordManagementSystem.Models
{
    public class StorageLocation
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public string? CreatedBy { get; set; }
        public int? FarmId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}