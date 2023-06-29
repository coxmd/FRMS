using System.ComponentModel.DataAnnotations;

namespace FarmRecordManagementSystem.Models
{
    public class ReportFormViewModel
    {
        public List<Farms> Farms { get; set; }
        public List<ReportTypes> ReportTypes { get; set; }
        public int SelectedFarmId { get; set; }
        public int SelectedReportTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}