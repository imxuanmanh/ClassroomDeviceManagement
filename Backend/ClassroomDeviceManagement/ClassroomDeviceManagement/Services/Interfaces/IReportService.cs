using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IReportService
    {
        Task<bool> AddReportAsync(Report report);
        Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(int statusId);
        //Task<bool> UpdateReportStatusAsync(int reportId, int newStatusId);
    }
}
