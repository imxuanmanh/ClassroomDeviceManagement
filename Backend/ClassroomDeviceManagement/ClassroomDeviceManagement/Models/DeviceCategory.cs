namespace ClassroomDeviceManagement.Models
{
    public class DeviceCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public DeviceCategory(string name)
        {
            Name = name;
        }
        public DeviceCategory(int id, string name)
        {
            Id = id;
        }
        
    }
}
