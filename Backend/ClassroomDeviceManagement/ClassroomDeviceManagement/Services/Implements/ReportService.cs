using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Interfaces;

namespace ClassroomDeviceManagement.Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IDeviceInstanceRepository _deviceInstanceRepository;

        public ReportService(IReportRepository reportRepository, IDeviceInstanceRepository deviceInstanceRepository)
        {
            _reportRepository = reportRepository;
            _deviceInstanceRepository = deviceInstanceRepository;
        }

        // Thêm report mới (mặc định status = New)
        public async Task<bool> AddReportAsync(Report report)
        {
            return await _reportRepository.AddReportAsync(report);
        }

        // Lấy danh sách report theo trạng thái
        public async Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(ReportStatus status)
        {
            return await _reportRepository.GetReportsByStatusAsync(status);
        }
        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            return await _reportRepository.GetByIdAsync(reportId);
        }


        // Bắt đầu xử lý report: chuyển report -> InProgress, instance -> Maintenance
        public async Task<bool> StartProcessingReportAsync(int reportId, int instanceId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.Status != ReportStatus.New)
            {
                Console.WriteLine("Report khong ton tai hoac khong phai new");
                return false; // 
            }


            var instance = await _deviceInstanceRepository.GetByIdAsync(instanceId);
            // if (instance == null || instance.StatusId != InstanceStatus.Available)
            if (instance == null)
            {
                Console.WriteLine("Instance khong hop le");
                return false; // 
            }

            // Cập nhật trạng thái
            var reportUpdated = await _reportRepository.UpdateReportStatusAsync(reportId, ReportStatus.InProgress);
            var instanceUpdated = await _deviceInstanceRepository.ChangeInstanceStatus(instanceId, InstanceStatus.Maintenance);

            if (reportUpdated == false)
            {
                Console.WriteLine("Cap nhat trang thai bao cao that bai");

            }

            if (instanceUpdated == false)
            {
                Console.WriteLine("Cap nhat trang thai instance that bai");

            }

            return reportUpdated && instanceUpdated;
        }

        public async Task<bool> CancelReportAsync(int reportId)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.Status != ReportStatus.New)
                return false;

            return await _reportRepository.UpdateReportStatusAsync(reportId, ReportStatus.Cancel);
        }


        // Hoàn tất report, isSuccess = true -> bảo trì thành công, false -> thất bại
        public async Task<bool> CompleteReportAsync(int reportId, bool isSuccess)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null || report.Status != ReportStatus.InProgress)
                return false; // report không hợp lệ

            var instance = await _deviceInstanceRepository.GetByIdAsync(report.InstanceId);
            if (instance == null) // || instance.StatusId != InstanceStatus.Maintenance)
                return false; // instance không hợp lệ

            // Cập nhật trạng thái báo cáo
            var reportUpdated = await _reportRepository.UpdateReportStatusAsync(reportId, isSuccess ? ReportStatus.Resolved : ReportStatus.Failed);

            if (reportUpdated == false) Console.WriteLine("Cap nhat trang thai report that bai");

            var newInstanceStatus = isSuccess ? InstanceStatus.Available : InstanceStatus.Broken;

            // Cập nhật trạng thái thiết bị
            var instanceUpdated = await _deviceInstanceRepository.ChangeInstanceStatus(instance.InstanceId, newInstanceStatus);

            if (instanceUpdated == false) Console.WriteLine("Cap nhat trang thai instance that bai");

            Console.WriteLine(reportUpdated && instanceUpdated);

            return reportUpdated && instanceUpdated;
        }
    }
}
