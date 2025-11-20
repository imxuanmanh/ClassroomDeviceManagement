namespace ClassroomDeviceManagement.ViewModels.BorrowRequest
{
    public class ApprovedRequestViewModel
    {
        public int RequestId {  get; set; }
        public string Borrower { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string InstanceCode { get; set; } = string.Empty;
        public DateTime RequestDate {  get; set; }
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; }
        public DateTime ApprovedDate { get; set; } = DateTime.Now;
        public string UsageLocation { get; set; } = string.Empty;
        
        public ApprovedRequestViewModel() { }
    }
}
