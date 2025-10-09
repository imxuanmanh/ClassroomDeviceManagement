namespace ClassroomDeviceManagement.Dto
{
    public enum UserRole
    {
        Admin = 0,
        Teacher = 1,
        Student = 2
    }
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Student;

        public List<BorrowRequestDto> BorrowRequests { get; set; } = new List<BorrowRequestDto>();

        public User(string userName, string passwordHash, string fullName, string email, UserRole role)
        {
            UserName = userName;
            PasswordHash = passwordHash;
            FullName = fullName;
            Email = email;
            Role = role;
        }
    }
}
