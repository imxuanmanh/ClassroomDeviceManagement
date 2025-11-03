using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IDeviceModelRepository
    {
        Task<IEnumerable<DeviceModelDto>> GetAllByCategoryId(int id);
        Task<DeviceModelDto?> GetByIdAsync(int id);

        Task AddModelAsync(DeviceModel model);
        //Task UpdateModelAsync(DeviceModel model);
        //Task DeleteModelAsync(int Id);
    }
}
