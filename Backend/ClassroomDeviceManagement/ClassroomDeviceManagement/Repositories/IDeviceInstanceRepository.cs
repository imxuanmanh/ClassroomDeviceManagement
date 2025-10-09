using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories
{
    public interface IDeviceInstanceRepository
    {
        Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id);
        // Task<DeviceInstanceDto?> GetInstanceByCodeAsync(string code);
        Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance);
        // Task UpdateInstanceAsync(DeviceInstanceDto instance);
        // Task DeleteInstanceAsync(int Id);

    }
}
