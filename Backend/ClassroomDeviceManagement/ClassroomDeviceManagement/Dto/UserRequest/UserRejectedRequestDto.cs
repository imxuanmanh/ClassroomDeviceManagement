namespace ClassroomDeviceManagement.Dto.UserRequest
{
    public class UserRejectedRequestDto
    {
        public int RequestId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public DateTime RequestDate {  get; set; }
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; }
        public string UsageLocation { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }
}
