using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IDeviceInstanceService
    {
        Task<IEnumerable<DeviceInstanceDto>> GetAllByModelIdAsync(int id);
        Task<DeviceInstanceDto?> AddInstanceAsync(DeviceInstance instance);
        
    }
}
