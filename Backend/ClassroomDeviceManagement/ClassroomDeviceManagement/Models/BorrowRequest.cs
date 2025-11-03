using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Models
{
    public class BorrowRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int InstanceId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string UsageLocation { get; set; } = string.Empty;
        public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;
        public string Purpose { get; set; } = string.Empty;

        public BorrowRequest() { }
    }
}
