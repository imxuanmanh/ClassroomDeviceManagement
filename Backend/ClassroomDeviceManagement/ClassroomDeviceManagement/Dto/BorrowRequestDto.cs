using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Dto
{
    public class BorrowRequestDto
    {
        public int RequestId {  get; set; }
        public string Borrower { get; set; } = string.Empty;
        public string InstanceCode { get; set; } = string.Empty; 
        public string DeviceName {  get; set; } = string.Empty;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string StorageLocation {  get; set; } = string.Empty;
        public string UsageLocation { get; set; } = string.Empty;
        public string Purpose {  get; set; } = string.Empty;
        public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;
    }
}
