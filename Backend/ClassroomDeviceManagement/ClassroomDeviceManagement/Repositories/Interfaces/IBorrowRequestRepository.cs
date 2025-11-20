using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.ViewModels;
using ClassroomDeviceManagement.ViewModels.BorrowRequest;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface IBorrowRequestRepository
    {
        Task<BorrowRequestDto?> AddBorrowRequestAsync(BorrowRequest request);
        Task<bool> DeleteBorrowRequestAsync(int requestId);
        Task<IEnumerable<PendingRequestViewModel>> GetPendingBorrowRequestsAsync();
        Task<IEnumerable<ApprovedRequestViewModel>> GetApprovedBorrowRequestsAsync();
        Task<IEnumerable<RejectedRequestViewModel>> GetRejectedBorrowRequestsAsync();
        Task<IEnumerable<ReturnedRequestViewModel>> GetReturnedBorrowRequestsAsync();
        Task<bool> UpdateBorrowRequestStatusAsync(int requestId, BorrowRequestStatus newStatus, DateTime? approvedDate = null, DateTime? returnDate = null);
        Task<int?> GetInstanceIdByRequestIdAsync(int requestId);
        Task<IEnumerable<BorrowedDeviceInstanceDto>> GetInstanceByUserId(int userId);
        Task<IEnumerable<BorrowRequestDto>> GetBorrowRequestsByUserIdAndStatusAsync(int userId, BorrowRequestStatus status);

    }
}
