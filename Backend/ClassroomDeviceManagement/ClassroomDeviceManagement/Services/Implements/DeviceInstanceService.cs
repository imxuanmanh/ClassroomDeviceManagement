using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Interfaces;

namespace ClassroomDeviceManagement.Services.Implements
{
    public class DeviceInstanceService : IDeviceInstanceService
    {
        private readonly IDeviceInstanceRepository _deviceInstanceRepository;
        public DeviceInstanceService(IDeviceInstanceRepository deviceInstanceRepository)
        {
            _deviceInstanceRepository = deviceInstanceRepository;
        }
        public async Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id)
        {
            return await _deviceInstanceRepository.GetAllByModelIdAsync(id);
        }

        public async Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance)
        {
            return await _deviceInstanceRepository.AddInstanceAsync(instance);
        }
    }
}
