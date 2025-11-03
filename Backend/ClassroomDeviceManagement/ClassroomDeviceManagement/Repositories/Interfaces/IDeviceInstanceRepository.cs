using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IDeviceInstanceRepository
    {
        Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id);
        Task<int?> GetAvailableInstanceByModelId(int id);
        Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance);

        Task<bool> ChangeInstanceStatus(int instanceId, InstanceStatus newStatus);
        // Task UpdateInstanceAsync(DeviceInstanceDto instance);
        // Task DeleteInstanceAsync(int Id);

    }
}
