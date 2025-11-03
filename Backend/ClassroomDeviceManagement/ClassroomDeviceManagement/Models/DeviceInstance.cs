using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Models
{
    public class DeviceInstance
    {
        public int InstanceId {  get; set; }
        public string InstanceCode {  get; set; } = string.Empty;
        public int ModelId {  get; set; }
        public InstanceStatus StatusId {  get; set; }
        public string CurrentLocation { get; set; } = string.Empty;

        public DeviceInstance() { }

    }
}
