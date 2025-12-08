using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<bool> AddReportAsync(Report report);
        Task<Report?> GetByIdAsync(int reportId);
        Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(ReportStatus status);
        Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus newStatus);
    }
}
