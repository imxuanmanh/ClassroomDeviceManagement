using Microsoft.Data.SqlClient;

namespace ClassroomDeviceManagement.Managers
{
    public interface IDbManager
    {
        Task ExecuteQueryAsync(string query, Func<SqlDataReader, Task> handler, params SqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters);
    }
}
