using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories;

namespace ClassroomDeviceManagement.Services
{
    public class DeviceInstanceService : IDeviceInstanceService
    {
        private readonly IDeviceInstanceRepository _repository;
        public DeviceInstanceService(IDeviceInstanceRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id)
        {
            return await _repository.GetAllByModelIdAsync(id);
        }

        public async Task<DeviceInstanceDto> AddInstanceAsync(DeviceInstance instance)
        {
            return await _repository.AddInstanceAsync(instance);
        }
    }
}
