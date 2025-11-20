namespace ClassroomDeviceManagement.Dto
{
    public class BorrowedDeviceInstanceDto
    {
        public int InstanceId {  get; set; }
        public string InstanceCode {  get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;

    }
}
