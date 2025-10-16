using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;

namespace ClassroomDeviceManagement.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto?> AddUserAsync(UserDto user);
        Task<User?> GetUserByUsernameAsync(string username); 
    }
}
