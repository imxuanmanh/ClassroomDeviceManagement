using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Enums;
using Microsoft.AspNetCore.Mvc;
using ClassroomDeviceManagement.Services;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using ClassroomDeviceManagement.Services.Implements;
using Microsoft.AspNetCore.Authorization;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller xử lý các chức năng thông báo người dùng.
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo theo Id người dùng.
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllNotificationsByUserId([FromRoute] int userId)
        {
            // Lấy tất cả thông báo cho userId
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

            // Kiểm tra nếu không có thông báo nào
            if (notifications == null || !notifications.Any())
            {
                return NotFound("Không có thông báo nào.");
            }

            return Ok(notifications);  // Trả về danh sách thông báo
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        [HttpPatch("{notificationId}/seen")]
        public async Task<IActionResult> MarkAsSeen(int notificationId)
        {
            var success = await _notificationService.MarkAsSeenAsync(notificationId);

            if (success)
                return NoContent();

            return NotFound();
        }
        [HttpPatch("{notificationId}/delete")]
        public async Task<IActionResult> MarkAsDeleted(int notificationId)
        {
            var success = await _notificationService.MarkAsDeletedAsync(notificationId);

            if (success)
                return NoContent();

            return NotFound();
        }


        /// <summary>
        /// Đăng kí SSE
        /// </summary>
        [HttpGet("subscribe/{userId}")]
        public async Task Subscribe(int userId, CancellationToken cancellationToken)
        {
            // Gọi sang Service, truyền Response và Token vào
            await _notificationService.SubscribeAsync(userId, Response, cancellationToken);
        }

    }
}
