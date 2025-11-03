using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.ViewModels;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<IEnumerable<UserViewModel>> GetAllUsers();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<UserDto?> AuthenticateAsync(string username, string password);
    }
}
