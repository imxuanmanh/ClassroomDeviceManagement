using Microsoft.Data.SqlClient;

namespace ClassroomDeviceManagement.Managers
{
    public interface IDbManager
    {
        Task ExecuteQueryAsync(string query, Func<SqlDataReader, Task> handler, params SqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters);

        Task<Dictionary<int, T>> ExecuteQueryAsync<T>(string sql, params SqlParameter[] parameters) where T : new();
        Task<T> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters);
        Task<bool> ExistsAsync(string sql, params SqlParameter[] parameters);
    }
}
