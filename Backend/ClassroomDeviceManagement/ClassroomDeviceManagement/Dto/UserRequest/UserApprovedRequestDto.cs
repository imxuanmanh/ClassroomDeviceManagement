namespace ClassroomDeviceManagement.Dto.UserRequest
{
    public class UserApprovedRequestDto
    {
        public int RequestId { get; set; }
        public string InstanceCode { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;
    }
}
