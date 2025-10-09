namespace ClassroomDeviceManagement.Dto
{
    public enum BorrowRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Returned = 3
    }

    public class BorrowRequestDto
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }    
        public int InstanceId { get; set; } 
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? ApprovedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string UsageLocation { get; set; } = string.Empty;
        public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;

        // Constructor rút gọn (chỉ khi tạo request mới)
        public BorrowRequestDto(int userId, int instanceId, string usageLocation)
        {
            UserId = userId;
            InstanceId = instanceId;
            UsageLocation = usageLocation;
            RequestDate = DateTime.Now;
            Status = BorrowRequestStatus.Pending;
        }

        public BorrowRequestDto(
            int requestId,
            int userId,
            int instanceId,
            DateTime requestDate,
            DateTime? approvedDate,
            DateTime? returnDate,
            string usageLocation,
            BorrowRequestStatus status
        )
        {
            RequestId = requestId;
            UserId = userId;
            InstanceId = instanceId;
            RequestDate = requestDate;
            ApprovedDate = approvedDate;
            ReturnDate = returnDate;
            UsageLocation = usageLocation;
            Status = status;
        }
    }
}
