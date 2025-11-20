using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Dto.UserRequest;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.ViewModels.BorrowRequest;

namespace ClassroomDeviceManagement.Services.Interfaces
{
    public interface IBorrowRequestService
    {
        Task<IEnumerable<PendingRequestViewModel>> GetPendingBorrowRequestsAsync();
        Task<IEnumerable<ApprovedRequestViewModel>> GetApprovedBorrowRequestsAsync();
        Task<IEnumerable<RejectedRequestViewModel>> GetRejectedBorrowRequestsAsync();
        Task<IEnumerable<ReturnedRequestViewModel>> GetReturnedBorrowRequestsAsync();
        Task<BorrowRequestDto?> CreateBorrowRequestAsync(CreateBorrowRequestDto requestDto);
        Task<bool> DeleteBorrowRequestAsync(int requestId);
        
        Task<bool> ApproveBorrowRequestAsync(int requestId);
        Task<bool> RejectBorrowRequestAsync(int requestId);
        Task<bool> ReturnBorrowRequestAsync(int requestId);
        Task<IEnumerable<UserPendingRequestDto>> GetUserPendingRequestAsync(int userId);
        Task<IEnumerable<UserApprovedRequestDto>> GetUserApprovedRequestAsync(int userId);
        Task<IEnumerable<UserRejectedRequestDto>> GetUserRejectedRequestAsync(int userId);
    }
}
