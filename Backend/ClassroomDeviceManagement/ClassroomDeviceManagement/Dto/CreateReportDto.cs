namespace ClassroomDeviceManagement.Dto
{
    public class CreateReportDto
    {
        public int UserId {  get; set; }
        public int InstanceId {  get; set; }
        public string Description { get; set; } = string.Empty;
        // public string ImagePath { get; set; } = string.Empty;
    }
}
