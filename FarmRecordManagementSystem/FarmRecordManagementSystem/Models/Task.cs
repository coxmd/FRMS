namespace FarmRecordManagementSystem.Models
{
    public class Task
    {
        public int? Id { get; set; }

        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
    }
}