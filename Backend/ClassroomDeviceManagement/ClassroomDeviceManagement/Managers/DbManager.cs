using ClassroomDeviceManagement.Repositories;
using Microsoft.Data.SqlClient;

namespace ClassroomDeviceManagement.Managers
{
    public class DbManager : IDbManager
    {
        private readonly string _connectionString;

        public DbManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database")
                ?? throw new ArgumentNullException("Connection string 'Database' not found.");
        }

        /// <summary>
        /// Thực thi SELECT hoặc query trả về dữ liệu, xử lý dữ liệu bằng handler async.
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="handler">Delegate async xử lý SqlDataReader</param>
        /// <param name="parameters">Tham số SQL (tùy chọn)</param>
        public async Task ExecuteQueryAsync(string query, Func<SqlDataReader, Task> handler, params SqlParameter[] parameters)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            try
            {
                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();
                await handler(reader);
            }
            catch (SqlException sqlEx)
            {
                // Log error message for more detailed info
                throw new Exception($"SQL Error when executing query: {query}, Error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"Error when executing query: {query}", ex);
            }
        }

        /// <summary>
        /// Thực thi INSERT/UPDATE/DELETE và trả về số dòng ảnh hưởng
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số SQL (tùy chọn)</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            try
            {
                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                // Log error message for more detailed info
                throw new Exception($"SQL Error when executing query: {query}, Error: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw new Exception($"Error when executing query: {query}", ex);
            }
        }
    }
}
