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
        public string UsageLocation { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;

        public PendingRequestViewModel() { }
    }
}
