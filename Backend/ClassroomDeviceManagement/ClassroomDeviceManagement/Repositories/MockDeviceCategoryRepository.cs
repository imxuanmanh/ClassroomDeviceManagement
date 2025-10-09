using ClassroomDeviceManagement.Dto;

namespace ClassroomDeviceManagement.Repositories
{
    public class MockDeviceCategoryRepository 
    {
        List<DeviceCategoryDto> _devices = new List<DeviceCategoryDto>
        {
            new DeviceCategoryDto { Id = 1, Name = "Laptop" },
            new DeviceCategoryDto { Id = 2, Name = "Tablet" },
            new DeviceCategoryDto { Id = 3, Name = "Projector" },
            new DeviceCategoryDto { Id = 4, Name = "Camera" },
            new DeviceCategoryDto { Id = 5, Name = "Speaker" }
        };

        public Task<IEnumerable<DeviceCategoryDto>> GetAllCategoryAsync()
        {
            return Task.FromResult<IEnumerable<DeviceCategoryDto>>(_devices);
        }
        // Trả về Task<DeviceCategory> tạm thời
        public Task<DeviceCategoryDto> GetByIdAsync(int id)
        {
            // Tạm thời trả về null
            return Task.FromResult<DeviceCategoryDto>(null);
        }

        // Trả về Task, tạm thời chưa làm gì
        public Task AddCategoryAsync(DeviceCategoryDto category)
        {
            return Task.CompletedTask;
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
