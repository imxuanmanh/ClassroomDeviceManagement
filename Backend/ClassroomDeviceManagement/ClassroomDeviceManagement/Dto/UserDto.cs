using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Dto
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole RoleId { get; set; }

        public UserDto() { }
    }
}
