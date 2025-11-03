using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Helper;
using Microsoft.Data.SqlClient;
using System.Reflection;

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

        // =============================================================================================================

        // Thực thi query trả về Dictionary<int, T> với key là Id property (int)
        public async Task<Dictionary<int, T>> ExecuteQueryAsync<T>(string sql, params SqlParameter[] parameters) where T : new()
        {
            var result = new Dictionary<int, T>();
            var keyProperty = typeof(T).GetProperty("Id");
            if (keyProperty == null)
                throw new InvalidOperationException($"Type {typeof(T).Name} không có property 'Id'.");
            if (keyProperty.PropertyType != typeof(int) && keyProperty.PropertyType != typeof(int?))
                throw new InvalidOperationException($"Property 'Id' của type {typeof(T).Name} phải là int hoặc int?.");

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            while (await reader.ReadAsync())
            {
                var obj = new T();

                foreach (var prop in props)
                {
                    if (!reader.HasColumn(prop.Name)) continue;

                    var value = reader[prop.Name];
                    if (value is DBNull)
                    {
                        prop.SetValue(obj, null);
                    }
                    else
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(value, targetType));
                    }
                }

                var keyValue = keyProperty.GetValue(obj);
                if (keyValue != null)
                {
                    var key = (int)keyValue;
                    if (!result.ContainsKey(key))
                        result.Add(key, obj);
                }
            }

            return result;
        }

        // Thực thi query trả về một giá trị đơn (scalar)
        public async Task<T?> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters) where T : struct
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            if (parameters != null && parameters.Length > 0)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            object? result = await command.ExecuteScalarAsync();

            if (result == null || result is DBNull)
                return null;  // trả về null nếu không có kết quả

            return (T)Convert.ChangeType(result, typeof(T));
        }


        // Kiểm tra tồn tại bản ghi trả về true/false
        public async Task<bool> ExistsAsync(string sql, params SqlParameter[] parameters)
        {
            int? count = await ExecuteScalarAsync<int>(sql, parameters);
            return (count ?? 0) > 0;
        }

    }
}
