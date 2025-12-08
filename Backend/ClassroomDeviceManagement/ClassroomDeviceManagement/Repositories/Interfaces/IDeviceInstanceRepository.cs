using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IDeviceInstanceRepository
    {
        Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id);
        Task<int?> GetAvailableInstanceByModelId(int modelId);
        Task<DeviceInstance?> GetByIdAsync(int instanceId);
        Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance);

        Task<bool> ChangeInstanceStatus(int instanceId, InstanceStatus newStatus);

    }
}
