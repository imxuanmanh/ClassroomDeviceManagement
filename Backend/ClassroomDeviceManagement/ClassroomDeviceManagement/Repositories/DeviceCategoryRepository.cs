using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Dto;
using Microsoft.Data.SqlClient;


namespace ClassroomDeviceManagement.Repositories
{
    public class DeviceCategoryRepository : IDeviceCategoryRepository
    {
        private readonly IDbManager _dbManager;

        public DeviceCategoryRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<IEnumerable<DeviceCategoryDto>> GetAllCategoriesAsync()
        {
            List<DeviceCategoryDto> categories = new List<DeviceCategoryDto>();
            await _dbManager.ExecuteQueryAsync(
                "SELECT id, name FROM device_category",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        categories.Add(new DeviceCategoryDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        });
                    }
                });
            return categories;
        }

        public async Task<DeviceCategoryDto?> GetCategoryByIdAsync(int id)
        {
            DeviceCategoryDto? result = null;
            await _dbManager.ExecuteQueryAsync(
                @"
                SELECT id, name 
                FROM device_category
                WHERE id = @categoryId;
                ",
                async reader =>
                {
                    if (await reader.ReadAsync()) // ✅ Phải có dòng này
                    {
                        result = new DeviceCategoryDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        };
                    }
                },
                new SqlParameter("@categoryId", id)
            );
            return result;  
        }
        
        public async Task<DeviceCategoryDto?> AddCategoryAsync(DeviceCategoryDto category)
        {
            DeviceCategoryDto? result = null;
            await _dbManager.ExecuteQueryAsync(
                @"
                INSERT INTO 
                    device_category (name) 

                OUTPUT
                    inserted.id, inserted.name

                VALUES
                    (@Name);
                
                ",
                async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        result = new DeviceCategoryDto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader.GetString(reader.GetOrdinal("name"))
                        };
                    }
                },
                new SqlParameter("@Name", category.Name)
                );
            return result;
        }

        public Task UpdateCategoryAsync(DeviceCategoryDto category)
        {
            return Task.CompletedTask;
        }

        public Task DeleteCategoryAsync(int id)
        {
            return Task.CompletedTask;
        }

    }
}
