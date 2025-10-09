using ClassroomDeviceManagement.Dto;

namespace ClassroomDeviceManagement.Repositories
{
    public interface IDeviceCategoryRepository
    {
        Task<IEnumerable<DeviceCategoryDto>> GetAllCategoriesAsync();
        Task<DeviceCategoryDto?> GetCategoryByIdAsync(int id);
        Task<DeviceCategoryDto?> AddCategoryAsync(DeviceCategoryDto category);
        Task UpdateCategoryAsync(DeviceCategoryDto category);
        Task DeleteCategoryAsync(int id);

    }
}
