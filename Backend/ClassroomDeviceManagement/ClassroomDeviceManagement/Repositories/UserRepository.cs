using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Helper;

namespace ClassroomDeviceManagement.Repositories
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
                        user.Email = reader.GetString(reader.GetOrdinal("email"));
                        user.RoleId = reader.GetInt32(reader.GetOrdinal("role_id"));
                    }
                },
                new SqlParameter("@username", username)
                );

            return user;
        }


    }
}
