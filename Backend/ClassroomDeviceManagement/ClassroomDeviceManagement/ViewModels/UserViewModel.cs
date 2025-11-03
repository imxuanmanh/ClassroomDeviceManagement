using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        public UserViewModel(){}

    }
}