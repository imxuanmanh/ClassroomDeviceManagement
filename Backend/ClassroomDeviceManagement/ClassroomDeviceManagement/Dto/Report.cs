namespace ClassroomDeviceManagement.Dto
{
    public enum ReportStatus
    {
        Open = 0,
        InProgress,
        Resolved
    }

    public class Report
    {
        public int ReportId { get; set; }
        public int UserId { get; set; }   
        public int InstanceId { get; set; }  
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public ReportStatus Status { get; set; } = ReportStatus.Open;

        public string? RejectReason {  get; set; }

        // Constructor dùng khi tạo report mới
        public Report(int userId, int instanceId, string description, string imagePath)
        {
            UserId = userId;
            InstanceId = instanceId;
            Description = description;
            ImagePath = imagePath;
            ReportDate = DateTime.Now;
            Status = ReportStatus.Open;
        }

        public Report(int reportId, int userId, int instanceId, string description, string imagePath, DateTime reportDate, ReportStatus status)
        {
            ReportId = reportId;
            UserId = userId;
            InstanceId = instanceId;
            Description = description;
            ImagePath = imagePath;
            ReportDate = reportDate;
            Status = status;
        }
    }
}
