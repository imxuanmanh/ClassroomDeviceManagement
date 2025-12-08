using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class DeviceInstanceRepository : IDeviceInstanceRepository
    {
        private readonly IDbManager _dbManager;
        public DeviceInstanceRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }
        public async Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id)
        {
            List<DeviceInstanceDto> instances = new List<DeviceInstanceDto>();

            await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    di.instance_id AS InstanceId,
                    di.instance_code AS InstanceCode,
                    di.status_id AS StatusId,
                    ISNULL(di.current_location, '') AS CurrentLocation,
                    CASE 
                        WHEN di.status_id = 2 AND br.user_id IS NOT NULL THEN ISNULL(u.fullname, '')
                        ELSE REPLICATE('-', 3) 
                    END AS Borrower,
                    CASE 
                        WHEN di.status_id = 2 AND br.request_date IS NOT NULL THEN
                            CONVERT(varchar(10), br.request_date, 103) -- dd/MM/yyyy
                            + N' (Tiết '
                            + CAST(br.start_period AS varchar(10))
                            + ' - '
                            + CAST(br.end_period AS varchar(10))
                            + ')'
                        ELSE REPLICATE('-', 3) 
                    END AS UsageDuration
                FROM device_instance di
                LEFT JOIN borrow_request br ON br.instance_id = di.instance_id
                LEFT JOIN [user] u ON br.user_id = u.user_id
                WHERE di.model_id = @modelId;
                ",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        instances.Add(new DeviceInstanceDto
                        {
                            InstanceId = reader.GetInt32(reader.GetOrdinal("InstanceId")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("InstanceCode")),
                            StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                            CurrentLocation = reader.GetString(reader.GetOrdinal("CurrentLocation")),
                            Borrower = reader.GetString(reader.GetOrdinal("Borrower")),
                            UsageDuration = reader.GetString(reader.GetOrdinal("UsageDuration"))
                        });
                    }
                },
                new SqlParameter("@modelId", id)
                );
            return instances;
        }

        public async Task<int?> GetAvailableInstanceByModelId(int modelId)
        {
            return await _dbManager.ExecuteScalarAsync<int>(
                @"
                SELECT TOP 1 
                    di.instance_id
    
                FROM 
                    device_instance di

                JOIN
                    instance_status s ON di.status_id = s.id

                WHERE 
                    di.model_id = @modelId

                AND s.name = 'Available';
                ",
                new SqlParameter("@modelId", modelId)
                );
        }

        public async Task<DeviceInstance?> GetByIdAsync(int instanceId)
        {
            DeviceInstance? result = null;

            await _dbManager.ExecuteQueryAsync(
                @"SELECT * FROM device_instance WHERE instance_id = @instanceId",
                async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        result = new DeviceInstance
                        {
                            InstanceId = reader.GetInt32(reader.GetOrdinal("instance_id")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("instance_code")),
                            ModelId = reader.GetInt32(reader.GetOrdinal("model_id")),
                            StatusId = (InstanceStatus)reader.GetInt32(reader.GetOrdinal("status_id")),
                            CurrentLocation = reader.GetString(reader.GetOrdinal("current_location"))
                        };
                    }
                },
                new SqlParameter("@instanceId", instanceId)
            );

            return result;
        }


        public async Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance)
        {
            DeviceInstanceDto? result = null;
            await _dbManager.ExecuteQueryAsync(
                @"
                INSERT INTO 
                    device_instance(instance_code, model_id, status_id, current_location)
                
                OUTPUT
                    inserted.instance_id, inserted.instance_code, inserted.status_id, inserted.current_location

                VALUES
                    (@instanceCode, @modelId, @statusId, @currentLocation);
                ",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        result = new DeviceInstanceDto
                        {
                            InstanceId = reader.GetInt32(reader.GetOrdinal("instance_id")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("instance_code")),
                            StatusId = reader.GetInt32(reader.GetOrdinal("status_id")),
                            CurrentLocation = reader.GetString(reader.GetOrdinal("current_location"))
                        };
                    }
                },
                new SqlParameter("@instanceCode", instance.InstanceCode),
                new SqlParameter("@modelId", instance.ModelId),
                new SqlParameter("@statusId", 1),
                new SqlParameter("@currentLocation", instance.CurrentLocation)
                );
            return result;
        }

        public async Task<bool> ChangeInstanceStatus(int instanceId, InstanceStatus newStatus)
        {
            try
            {
                int row = await _dbManager.ExecuteNonQueryAsync(
                    @"
                        UPDATE 
                            device_instance
                        SET 
                            status_id = @statusId
                        WHERE
                            instance_id = @instanceId;
                    ",
                    new SqlParameter("@statusId", (int)newStatus),
                    new SqlParameter("@instanceId", instanceId)
                    );
                return row > 0;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }

        }
    }
}
