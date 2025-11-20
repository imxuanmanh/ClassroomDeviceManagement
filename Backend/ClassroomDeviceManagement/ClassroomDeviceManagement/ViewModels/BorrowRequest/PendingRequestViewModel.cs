using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.ViewModels.BorrowRequest
{
    public class PendingRequestViewModel
    {
        public int RequestId { get; set; }
        public string Borrower { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string InstanceCode { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; }
        public string UsageLocation { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;

        public PendingRequestViewModel() { }
    }
}
