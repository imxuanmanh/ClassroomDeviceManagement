using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<bool> AddReportAsync(Report report);
        Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(int statusId);
        Task<bool> UpdateReportStatusAsync(int reportId, int newStatusId);
    }
}
