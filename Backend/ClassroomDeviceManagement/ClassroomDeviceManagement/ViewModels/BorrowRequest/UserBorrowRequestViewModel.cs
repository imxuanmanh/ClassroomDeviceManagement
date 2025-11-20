namespace ClassroomDeviceManagement.ViewModels.BorrowRequest
{
    public class UserBorrowRequestViewModel
    {
        public int RequestId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string InstanceCode { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Period { get; set; } = string.Empty;
        public int Status { get; set; }
    }

}
