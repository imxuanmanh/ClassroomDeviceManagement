using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller quản lý báo cáo hư hỏng
    /// </summary>
    [Route("api/reports")]
    [ApiController]
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

    }
}
