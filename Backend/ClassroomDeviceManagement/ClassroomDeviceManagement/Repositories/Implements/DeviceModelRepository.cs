using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Repositories.Interfaces;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class DeviceModelRepository : IDeviceModelRepository
    {
        private readonly IDbManager _dbManager;

        public DeviceModelRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<IEnumerable<DeviceModelDto>> GetAllByCategoryId(int id)
        {
            List<DeviceModelDto> models = new List<DeviceModelDto>();

            await _dbManager.ExecuteQueryAsync(
                @"
                SELECT
                	model.model_id AS id,
                	model.model_name AS name,
                	model.specifications AS specifications,
                	model.storage_location AS storage_location,
                	COUNT(instance.instance_id) as total_quantity,
                	COUNT(CASE WHEN instance.status_id = 1 THEN 1 END) AS available_quantity
                
                FROM
                	device_model model
                
                LEFT JOIN 
                	device_instance instance ON model.model_id = instance.model_id
                
                WHERE
                	model.category_id = @categoryId
                
                GROUP BY
                	model.model_id,
                	model.model_name, 
                    model.specifications, 
                    model.storage_location;",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        models.Add(new DeviceModelDto
                        {
                            ModelId = reader.GetInt32(reader.GetOrdinal("id")),
                            ModelName = reader.GetString(reader.GetOrdinal("name")),
                            Specifications = reader.GetString(reader.GetOrdinal("specifications")),
                            StorageLocation = reader.GetString(reader.GetOrdinal("storage_location")),
                            TotalQuantity = reader.GetInt32(reader.GetOrdinal("total_quantity")),
                            AvailableQuantity = reader.GetInt32(reader.GetOrdinal("available_quantity"))
                        });
                    }
                },
                new SqlParameter("@categoryId", id)
                );

            return models;
        }
        
        public async Task<DeviceModelDto?> GetByIdAsync(int id)
        {
            DeviceModelDto device = new DeviceModelDto();

            await _dbManager.ExecuteQueryAsync(
                @"
                SELECT
	                model.model_id AS id,
	                model.model_name AS name,
	                model.specifications AS specifications,
	                model.storage_location AS storage_location,
	                COUNT(instance.instance_id) as total_quantity,
	                COUNT(CASE WHEN instance.status_id = 1 THEN 1 END) AS available_quantity

                FROM
	                device_model model

                JOIN 
	                device_instance instance ON model.model_id = instance.model_id

                WHERE
	                model.model_id = @modelId

                GROUP BY
	                model.model_id,
	                model.model_name, 
                    model.specifications, 
                    model.storage_location;
                ",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        device.ModelId = reader.GetInt32(reader.GetOrdinal("id"));
                        device.ModelName = reader.GetString(reader.GetOrdinal("name"));
                        device.Specifications = reader.GetString(reader.GetOrdinal("specifications"));
                        device.StorageLocation = reader.GetString(reader.GetOrdinal("storage_location"));
                        device.TotalQuantity = reader.GetInt32(reader.GetOrdinal("total_quantity"));
                        device.AvailableQuantity = reader.GetInt32(reader.GetOrdinal("available_quantity"));
                    }
                },
                new SqlParameter("@modelId", id)
                );
            return device;
        }

        public async Task AddModelAsync(DeviceModel model)
        {
            await _dbManager.ExecuteNonQueryAsync(
                @"
                INSERT INTO 
                    device_model (model_name, category_id, specifications, storage_location)

                VALUES 
                    (@modelName, @categoryId, @specifications, @storageLocation)",
                new SqlParameter("@modelName", model.ModelName),
                new SqlParameter("@categoryId", model.CategoryId),
                new SqlParameter("@specifications", model.Specifications),
                new SqlParameter("@storageLocation", model.StorageLocation)
                );
        }

    }
}
