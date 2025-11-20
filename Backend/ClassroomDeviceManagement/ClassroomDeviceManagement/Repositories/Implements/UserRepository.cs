using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Helper;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.ViewModels;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbManager _dbManager;

        public UserRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<UserDto?> AddUserAsync(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password không được để trống");

            bool exists = await _dbManager.ExistsAsync(
                "SELECT COUNT(1) FROM [user] WHERE username = @username",
                new SqlParameter("@username", user.Username));

            if (exists)
                throw new InvalidOperationException("Username đã tồn tại");

            string passwordHash = PasswordHelper.HashPassword(user.Password);

            int affectedRows = await _dbManager.ExecuteNonQueryAsync(
                @"
            INSERT INTO [user] (username, password_hash, fullname, email, role_id)
            VALUES (@username, @passwordHash, @fullname, @email, @roleId);
        ",
                new SqlParameter("@username", user.Username),
                new SqlParameter("@passwordHash", passwordHash),
                new SqlParameter("@fullname", user.Fullname),
                new SqlParameter("@email", user.Email),
                new SqlParameter("@roleId", user.RoleId));

            if (affectedRows == 0)
                throw new Exception("Không thể thêm user. Kiểm tra role_id hoặc dữ liệu đầu vào.");

            user.Password = null;
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            User? user = null;

            await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    user_id, username, password_hash, fullname, email, role_id
            
                FROM
                    [user]

                WHERE
                    username = @username;
                ",

                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        user = new User();

                        user.Id = reader.GetInt32(reader.GetOrdinal("user_id"));
                        user.Username = reader.GetString(reader.GetOrdinal("username"));
                        user.PasswordHash = reader.GetString(reader.GetOrdinal("password_hash"));
                        user.Fullname = reader.GetString(reader.GetOrdinal("fullname"));
                        user.Email = reader.GetString(reader.GetOrdinal("email"));
                        user.RoleId = reader.GetInt32(reader.GetOrdinal("role_id"));
                    }
                },
                new SqlParameter("@username", username)
                );

            return user;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsers()
        {
            List<UserViewModel> users = new List<UserViewModel>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                    @"
                    SELECT
                        user_id AS UserId,
                        username AS Username,
                        fullname AS Fullname,
                        email AS Email,
                        role_id AS RoleId
                    FROM 
                        [user];
                    ",

                    async reader =>
                    {
                        int userIdIndex = reader.GetOrdinal("UserId");
                        int usernameIndex = reader.GetOrdinal("Username");
                        int fullnameIndex = reader.GetOrdinal("Fullname");
                        int emailIndex = reader.GetOrdinal("Email");
                        int roleIndex = reader.GetOrdinal("RoleId");

                        while (await reader.ReadAsync())
                        {
                            users.Add(new UserViewModel
                            {
                                UserId = reader.GetInt32(userIdIndex),
                                Username = reader.GetString(usernameIndex),
                                Fullname = reader.GetString(fullnameIndex),
                                Email = reader.GetString(emailIndex),
                                Role = (Enums.UserRole)reader.GetInt32(roleIndex)
                            });
                        }
                    }
                );
                return users;

            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }
        }
    
         
    }
}
