using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Services
{
    public class DeviceCategoryService : IDeviceCategoryService
    {
        private readonly IDeviceCategoryRepository _repository;

        public DeviceCategoryService(IDeviceCategoryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<DeviceCategoryDto>> GetAllDeviceCategories()
        {
            return _repository.GetAllCategoriesAsync();
        }

        public async Task<DeviceCategoryDto?> GetCategoryById(int id)
        {
            return await _repository.GetCategoryByIdAsync(id);
        }

        public async Task<DeviceCategoryDto?> AddCategoryAsync(DeviceCategoryDto deviceCategory)
        {
            return await _repository.AddCategoryAsync(deviceCategory);
        }
    }
}
