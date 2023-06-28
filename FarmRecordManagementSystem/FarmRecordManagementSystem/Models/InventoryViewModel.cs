namespace FarmRecordManagementSystem.Models
{
    public class InventoryViewModel
    {
        public int Id { get; set; }
        public Farms Farm { get; set; }
        public List<Inventory> Inventory { get; set; }
    }
}