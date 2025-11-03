using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Services.Interfaces;
using ClassroomDeviceManagement.ViewModels.BorrowRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{

    /// <summary>
    /// Controller để yêu cầu mượn thiết bị.
    /// </summary>
    [Route("api/borrow-requests")]
    [ApiController]
    public class BorrowRequestController : ControllerBase
    {
        private readonly IBorrowRequestService _borrowRequestService;

        public BorrowRequestController(IBorrowRequestService borrowRequestService)
        {
            _borrowRequestService = borrowRequestService;
        }

        /// <summary>
        /// Lấy ra các request chưa giải quyết
        /// </summary>
        [HttpGet("pending")] 
        public async Task<IActionResult> GetPendingBorrowRequests()
        {
            IEnumerable<PendingRequestViewModel> requests = await _borrowRequestService.GetPendingBorrowRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Lấy ra các request đã đồng ý
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedBorrowRequests()
        {
            IEnumerable<ApprovedRequestViewModel> requests = await _borrowRequestService.GetApprovedBorrowRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Lấy ra các request đã từ chối
        /// </summary>
        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedBorrowRequests()
        {
            IEnumerable<RejectedRequestViewModel> requests = await _borrowRequestService.GetRejectedBorrowRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Lấy ra các request đã trả lại
        /// </summary>
        [HttpGet("returned")]
        public async Task<IActionResult> GetReturnedBorrowRequests()
        {
            IEnumerable<ReturnedRequestViewModel> requests = await _borrowRequestService.GetReturnedBorrowRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Tạo một request mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] CreateBorrowRequestDto requestDto)
        {
            BorrowRequestDto? result = await _borrowRequestService.CreateBorrowRequestAsync(requestDto);

            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new
            {
                Message = "Không có thiết bị nào rảnh để mượn theo model này."
            });
        }

        /// <summary>
        /// Chấp nhận một request
        /// </summary>
        [HttpPatch("{requestId}/approve")]
        public async Task<IActionResult> ApproveBorrowRequest(int requestId)
        {
            bool success = await _borrowRequestService.ApproveBorrowRequestAsync(requestId);

            if (success)
            {
                return Ok(new { message = "Request approved successfully." });
            }

            return BadRequest(new { message = "Failed to approve the request." });
        }

        /// <summary>
        /// Từ chối một request
        /// </summary>
        [HttpPatch("{requestId}/reject")]
        public async Task<IActionResult> RejectBorrowRequest(int requestId)
        {
            bool success = await _borrowRequestService.RejectBorrowRequestAsync(requestId);

            if (success)
            {
                return Ok(new { message = "Request rejected successfully." });
            }

            return BadRequest(new { message = "Failed to reject the request." });
        }

        /// <summary>
        /// Thu hồi một request
        /// </summary>
        [HttpPatch("{requestId}/return")]
        public async Task<IActionResult> ReturnBorrowRequest(int requestId)
        {
            bool success = await _borrowRequestService.ReturnBorrowRequestAsync(requestId);

            if (success)
            {
                return Ok(new { message = "Request returned successfully." });
            }

            return BadRequest(new { message = "Failed to return the request." });
        }
    }
}
