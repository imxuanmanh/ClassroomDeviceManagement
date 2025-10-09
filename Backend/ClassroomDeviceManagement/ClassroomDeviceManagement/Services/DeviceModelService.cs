using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories;

namespace ClassroomDeviceManagement.Services
{
    public class DeviceModelService : IDeviceModelService
    {
        private readonly IDeviceModelRepository _repository;

        public DeviceModelService(IDeviceModelRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DeviceModelDto>> GetAllByCategoryId(int id)
        {
            return await _repository.GetAllByCategoryId(id);
        }

        public async Task<DeviceModelDto?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);

        }

        public async Task AddModelAsync(DeviceModel model)
        {
            await _repository.AddModelAsync(model);
        }
    }
}
