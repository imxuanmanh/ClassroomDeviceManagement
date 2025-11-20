using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Models
{

    public class Report
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }   
        public int InstanceId { get; set; }  
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public ReportStatus Status { get; set; } = ReportStatus.New;
    }
}
