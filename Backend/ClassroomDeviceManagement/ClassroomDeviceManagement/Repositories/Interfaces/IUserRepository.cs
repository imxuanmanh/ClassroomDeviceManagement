using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.ViewModels;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserViewModel>> GetAllUsers();
    }
}
