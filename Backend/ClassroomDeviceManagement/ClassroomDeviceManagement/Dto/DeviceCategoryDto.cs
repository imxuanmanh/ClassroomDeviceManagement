namespace ClassroomDeviceManagement.Dto
{
    public class DeviceCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public DeviceCategoryDto() { }
        public DeviceCategoryDto(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
