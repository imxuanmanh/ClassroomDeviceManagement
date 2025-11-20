using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller để quản lý người dùng
    /// </summary>
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBorrowRequestService _borrowRequestService;

        public UserController(IUserService userService, IBorrowRequestService borrowRequestService)
        {
            _userService = userService;
            _borrowRequestService = borrowRequestService;
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
    }
}
