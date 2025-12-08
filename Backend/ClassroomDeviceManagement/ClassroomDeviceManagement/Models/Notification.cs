namespace ClassroomDeviceManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool Seen { get; set; }
        public bool Deleted { get; set; }
    }

}