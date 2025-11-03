using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IDeviceModelService
    {
        Task<IEnumerable<DeviceModelDto>> GetAllByCategoryId(int id);
        Task<DeviceModelDto?> GetById(int id);
        Task AddModelAsync(DeviceModel model);
    }
}
