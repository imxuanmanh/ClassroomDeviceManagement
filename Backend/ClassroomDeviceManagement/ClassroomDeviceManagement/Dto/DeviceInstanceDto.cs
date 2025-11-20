using System.Text.Json;

namespace ClassroomDeviceManagement.Dto
{
    public class DeviceInstanceDto
    {
        public int InstanceId { get; set; }
        public string InstanceCode { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string CurrentLocation { get; set; } = string.Empty;
        
        public DeviceInstanceDto() { }
        public DeviceInstanceDto(string instanceCode, string currentLocation)
        {
            InstanceCode = instanceCode;
            CurrentLocation = currentLocation;
        }

        public DeviceInstanceDto(int instanceId, string instanceCode, int statusId, string currentLocation)
        {
            InstanceId = instanceId;
            InstanceCode = instanceCode;
            StatusId = statusId;
            CurrentLocation = currentLocation;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}

