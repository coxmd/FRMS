namespace FarmRecordManagementSystem.Models
{
    public class Tasks
    {
        public int? Id { get; set; }
        public int? FarmId { get; set; }

        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? AssignedTo { get; set; }
        public Boolean Completed { get; set; }

        public DateTime? DateStarted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public DateTime? DueDate { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}