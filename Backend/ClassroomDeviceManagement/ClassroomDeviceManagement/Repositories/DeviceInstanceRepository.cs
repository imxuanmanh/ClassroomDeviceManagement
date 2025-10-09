using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using Microsoft.Data.SqlClient;

namespace ClassroomDeviceManagement.Repositories
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
    }
}
