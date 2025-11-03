using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Repositories;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IDeviceCategoryService
    {
        Task<IEnumerable<DeviceCategoryDto>> GetAllDeviceCategories();
        Task<DeviceCategoryDto?> GetCategoryById(int id);

        Task<DeviceCategoryDto?> AddCategoryAsync(DeviceCategoryDto deviceCategory);
    }
}
