using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Helper;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Enums;

namespace ClassroomDeviceManagement.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserDto?> AddUserAsync(UserDto user)
        {
            return await _userRepository.AddUserAsync(user);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        /// <summary>
        /// Xác thực đăng nhập
        /// </summary>
        public async Task<UserDto?> AuthenticateAsync(string username, string password)
        {
            User? loginUser = await _userRepository.GetUserByUsernameAsync(username);

            if (loginUser == null)
            {
                return null;
            } 

            bool isPasswordValid = PasswordHelper.VerifyPassword(password, loginUser.PasswordHash);

            if (isPasswordValid)
            {
                return new UserDto
                {
                    Username = loginUser.Username,
                    Password = null,
                    Fullname = loginUser.Fullname,
                    Email = loginUser.Email,
                    RoleId = (Role)loginUser.RoleId
                };
            }

            return null;
        }
    }
}
