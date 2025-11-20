namespace ClassroomDeviceManagement.ViewModels.Report
{
    public class ReportViewModel
    {
        public int ReportId { get; set; }           
        public string UserFullName { get; set; } = string.Empty; 
        public string DeviceName { get; set; } = string.Empty;   
        public string InstanceCode { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty; 
        public string ImagePath { get; set; } = string.Empty;  
        public DateTime ReportDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
