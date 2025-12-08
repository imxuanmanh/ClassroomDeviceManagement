using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller quản lý báo cáo hư hỏng
    /// </summary>
    [Route("api/reports")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Tạo một báo cáo hư hỏng mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] CreateReportDto createReportDto, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No file uploaded!");
            }

            var ext = Path.GetExtension(image.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowed.Contains(ext))
                return BadRequest("Invalid file type");

            if (image.Length > 5 * 1024 * 1024)
                return BadRequest("File too large");

            string fileName = Guid.NewGuid() + ext;

            string savePath = Path.Combine("wwwroot/images", fileName);

            Directory.CreateDirectory("wwwroot/images");

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            string imageUrl = "/images/" + fileName;

            Report report = new Report
            {
                UserId = createReportDto.UserId,
                InstanceId = createReportDto.InstanceId,
                Description = createReportDto.Description,
                ImagePath = imageUrl
            };

            if (await _reportService.AddReportAsync(report))
            {
                return Ok("Gửi báo cáo thành công");
            }

            return BadRequest("Gửi báo cáo thất bại");
        }

        /// <summary>
        /// Lấy ra danh sách báo cáo theo trạng thái
        /// </summary>
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetReportByStatus([FromRoute] int statusId)
        {
            var reports = await _reportService.GetReportsByStatusAsync((Enums.ReportStatus)statusId);
            return Ok(reports);
        }

        /// <summary>
        /// Bắt đầu xử lý request
        /// </summary>
        [HttpPatch("{reportId}/process")]
        public async Task<IActionResult> StartProcessingReport([FromRoute] int reportId)
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            if (report == null)
                return NotFound("Report not found");

            var success = await _reportService.StartProcessingReportAsync(reportId, report.InstanceId);
            return success ? Ok("Report processing started") : BadRequest("Failed to start processing report");
        }

        /// <summary>
        /// Hủy bỏ báo cáo
        /// </summary>
        [HttpPatch("{reportId}/cancel")]
        public async Task<IActionResult> CancelReport([FromRoute] int reportId)
        {
            var success = await _reportService.CancelReportAsync(reportId);
            return success ? Ok("Report canceled") : BadRequest("Cannot cancel report");
        }

        /// <summary>
        /// Hoàn thành xử lý báo cáo
        /// </summary>
        [HttpPatch("{reportId}/complete")]
        public async Task<IActionResult> CompleteReport([FromRoute] int reportId, [FromQuery] bool isSuccess)  // client gửi true/false hoặc 1/0
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            if (report == null)
                return NotFound("Report not found");

            var success = await _reportService.CompleteReportAsync(reportId, isSuccess);


            Console.WriteLine("Cap nhat that bai. success = " + success);

            return success ? Ok("Report completed") : BadRequest("Failed to complete report");
        }
    }
}
