using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Implements;
using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller để quản lý người dùng
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBorrowRequestService _borrowRequestService;
        private readonly INotificationService _notificationService;

        public UserController(IUserService userService, IBorrowRequestService borrowRequestService, INotificationService notificationService)
        {
            _userService = userService;
            _borrowRequestService = borrowRequestService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsers();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu đang chờ của một người dùng
        /// </summary>
        [HttpGet("{userId}/borrow-requests/pending")]
        public async Task<IActionResult> GetUserPendingRequests(int userId)
        {
            var result = await _borrowRequestService.GetUserPendingRequestAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu được chấp nhận của một người dùng
        /// </summary>
        [HttpGet("{userId}/borrow-requests/approved")]
        public async Task<IActionResult> GetUserApprovedRequests(int userId)
        {
            var result = await _borrowRequestService.GetUserApprovedRequestAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách yêu cầu bị từ chối của một người dùng
        /// </summary>
        [HttpGet("{userId}/borrow-requests/rejected")]
        public async Task<IActionResult> GetUserRejectedRequests(int userId)
        {
            var result = await _borrowRequestService.GetUserRejectedRequestAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách thông báo của người dùng
        /// </summary>
        [HttpGet("{userId}/notifications")]
        public async Task<IActionResult> GetUserNotifications([FromRoute] int userId)
        {
            var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);

            return Ok(notifications);
        }


        /// <summary>
        /// Đánh dấu tất cả thông báo là đã đọc
        /// </summary>
        [HttpPatch("{userId}/notifications/seen")]
        public async Task<IActionResult> MarkAllNotificationsAsSeen([FromRoute] int userId)
        {
            var result = await _notificationService.MarkAllAsSeenAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo là đã đọc
        /// </summary>
        [HttpPatch("{userId}/notifications/deleted")]
        public async Task<IActionResult> MarkAllNotificationsAsDeleted([FromRoute] int userId)
        {
            var result = await _notificationService.MarkAllAsDeletedAsync(userId);
            return Ok(result);
        }
    }
}
