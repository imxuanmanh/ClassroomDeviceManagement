using ClassroomDeviceManagement.Dto;
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
                SELECT instance_id, instance_code, status_id, current_location

                FROM 
                    device_instance 

                WHERE 
                    model_id = @modelId;
                ",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        instances.Add(new DeviceInstanceDto
                        {
                            InstanceId = reader.GetInt32(reader.GetOrdinal("instance_id")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("instance_code")),
                            StatusId = reader.GetInt32(reader.GetOrdinal("status_id")),
                            CurrentLocation = reader.GetString(reader.GetOrdinal("current_location"))
                        });
                    }
                },
                new SqlParameter("@modelId", id)
                );
            return instances;
        }

        public async Task<int?> GetAvailableInstanceByModelId(int id)
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
                new SqlParameter("@modelId", id)
                );
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
