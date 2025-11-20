using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Interfaces;

namespace ClassroomDeviceManagement.Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<bool> AddReportAsync(Report report)
        {
           return await _reportRepository.AddReportAsync(report);
        }

        public async Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(int statusId)
        {
            return await _reportRepository.GetReportsByStatusAsync(statusId);
        }
        //public Task<bool> UpdateReportStatusAsync(int reportId, int newStatusId)
        //{
            
        //}
    }
}
