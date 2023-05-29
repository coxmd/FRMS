namespace FarmRecordManagementSystem.Models
{
    public class Harvest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public string? PriceSold { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}