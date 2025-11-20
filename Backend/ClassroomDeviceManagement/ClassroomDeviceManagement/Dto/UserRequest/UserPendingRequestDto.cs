namespace ClassroomDeviceManagement.Dto.UserRequest
{
    public class UserPendingRequestDto
    {
        public int RequestId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public byte StartPeriod { get; set; }
        public byte EndPeriod { get; set; } 
        public string UsageLocation { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
    }
}
