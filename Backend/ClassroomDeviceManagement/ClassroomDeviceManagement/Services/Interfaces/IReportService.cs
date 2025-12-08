using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IReportService
    {
        Task<bool> AddReportAsync(Report report);
        Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(ReportStatus status);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task<bool> StartProcessingReportAsync(int reportId, int instanceId);
        Task<bool> CancelReportAsync(int reportId);
        Task<bool> CompleteReportAsync(int reportId, bool isSuccess);

    }
}
