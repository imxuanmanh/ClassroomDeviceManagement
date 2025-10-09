namespace ClassroomDeviceManagement.Models
{
    public class DeviceInstance
    {
        public int InstanceId {  get; set; }
        public string InstanceCode {  get; set; } = string.Empty;
        public int ModelId {  get; set; }
        public int StatusId {  get; set; }
        public string CurrentLocation { get; set; } = string.Empty;

        public DeviceInstance() { }

        public DeviceInstance(string instanceCode, int modelId, int statusId, string currentLocation)
        {
            InstanceCode = instanceCode;
            ModelId = modelId;
            StatusId = statusId;
            CurrentLocation = currentLocation;
        }

        public DeviceInstance(int instanceId, string instanceCode, int modelId, int statusId, string currentLocation)
        {
            InstanceId = instanceId;
            InstanceCode = instanceCode;
            ModelId = modelId;
            StatusId = statusId;
            CurrentLocation = currentLocation;
        }
    }
}
