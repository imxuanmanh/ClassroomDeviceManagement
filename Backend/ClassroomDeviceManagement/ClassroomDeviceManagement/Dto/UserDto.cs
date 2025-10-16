using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Dto
{
    public enum UserRole
    {
        Admin = 0,
        Teacher = 1,
        Student = 2
    }
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Role RoleId { get; set; }

        public UserDto() { }
    }
}
