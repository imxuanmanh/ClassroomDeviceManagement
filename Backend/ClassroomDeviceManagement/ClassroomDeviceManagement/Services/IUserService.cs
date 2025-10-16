using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services
{
    public interface IUserService
    {
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<UserDto?> AuthenticateAsync(string username, string password);
    }
}
