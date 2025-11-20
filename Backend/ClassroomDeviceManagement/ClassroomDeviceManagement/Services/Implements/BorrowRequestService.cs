using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Enums;
using ClassroomDeviceManagement.Dto.UserRequest;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Services.Interfaces;
using ClassroomDeviceManagement.ViewModels.BorrowRequest;

namespace ClassroomDeviceManagement.Services.Implements
{
    public class BorrowRequestService : IBorrowRequestService
    {
        private readonly IBorrowRequestRepository _borrowRequestRepository;
        private readonly IDeviceInstanceRepository _deviceInstanceRepository;

        public BorrowRequestService(IBorrowRequestRepository borrowRequestRepository, IDeviceInstanceRepository deviceInstanceRepository)
        {
            _borrowRequestRepository = borrowRequestRepository;
            _deviceInstanceRepository = deviceInstanceRepository;
        }

        public async Task<BorrowRequestDto?> CreateBorrowRequestAsync(CreateBorrowRequestDto requestDto)
        {
            int? instanceId = await _deviceInstanceRepository.GetAvailableInstanceByModelId(requestDto.ModelId);

            if (instanceId == null)
            {
                return null;
            }

            BorrowRequest borrowRequest = new BorrowRequest
            {
                UserId = requestDto.UserId,
                InstanceId = instanceId.Value,
                UsageLocation = requestDto.UsageLocation,
                StartPeriod = requestDto.StartPeriod,
                EndPeriod = requestDto.EndPeriod,
                Purpose = requestDto.Purpose
            };

            return await _borrowRequestRepository.AddBorrowRequestAsync(borrowRequest);
        }

        public async Task<bool> DeleteBorrowRequestAsync(int requestId)
        {
            return await _borrowRequestRepository.DeleteBorrowRequestAsync(requestId);
        }

        public async Task<IEnumerable<PendingRequestViewModel>> GetPendingBorrowRequestsAsync()
        {
            return await _borrowRequestRepository.GetPendingBorrowRequestsAsync();
        }        
        public async Task<IEnumerable<ApprovedRequestViewModel>> GetApprovedBorrowRequestsAsync()
        {
            return await _borrowRequestRepository.GetApprovedBorrowRequestsAsync();
        }
        public async Task<IEnumerable<RejectedRequestViewModel>> GetRejectedBorrowRequestsAsync()
        {
            return await _borrowRequestRepository.GetRejectedBorrowRequestsAsync();
        }
        public async Task<IEnumerable<ReturnedRequestViewModel>> GetReturnedBorrowRequestsAsync()
        {
            return await _borrowRequestRepository.GetReturnedBorrowRequestsAsync();
        }

        public async Task<bool> ApproveBorrowRequestAsync(int requestId)
        {
            int? instanceId = await _borrowRequestRepository.GetInstanceIdByRequestIdAsync(requestId);

            if (instanceId == null) return false;

            bool updateInstanceStatus = await _deviceInstanceRepository.ChangeInstanceStatus(instanceId.Value, Enums.InstanceStatus.Borrowed);
            bool approvedRequest = await _borrowRequestRepository.UpdateBorrowRequestStatusAsync(requestId, Enums.BorrowRequestStatus.Approved, DateTime.Now, null);

            return updateInstanceStatus && approvedRequest;

            // return await _borrowRequestRepository.UpdateBorrowRequestStatusAsync(requestId, Enums.BorrowRequestStatus.Approved, DateTime.Now, null);
        }

        public async Task<bool> RejectBorrowRequestAsync(int requestId)
        {
             return await _borrowRequestRepository.UpdateBorrowRequestStatusAsync(requestId, Enums.BorrowRequestStatus.Rejected, null, null);   
        }
        
        public async Task<bool> ReturnBorrowRequestAsync(int requestId)
        {        
            int? instanceId = await _borrowRequestRepository.GetInstanceIdByRequestIdAsync(requestId);

            if (instanceId == null) return false;

            bool updateInstanceStatus = await _deviceInstanceRepository.ChangeInstanceStatus(instanceId.Value, InstanceStatus.Available);
            bool returnRequest = await _borrowRequestRepository.UpdateBorrowRequestStatusAsync(requestId, Enums.BorrowRequestStatus.Returned, null, DateTime.Now);

            return updateInstanceStatus && returnRequest;
        }

        public async Task<IEnumerable<UserPendingRequestDto>> GetUserPendingRequestAsync(int userId)
        {
            var pendingRequests = await _borrowRequestRepository.GetBorrowRequestsByUserIdAndStatusAsync(userId, Enums.BorrowRequestStatus.Pending);

            return pendingRequests.Select(r => new UserPendingRequestDto
            {
                RequestId = r.RequestId,
                DeviceName = r.DeviceName,
                StartPeriod = r.StartPeriod,
                EndPeriod = r.EndPeriod,
                UsageLocation = r.UsageLocation,
                Purpose = r.Purpose
            });
        }

        public async Task<IEnumerable<UserApprovedRequestDto>> GetUserApprovedRequestAsync(int userId)
        {
            var approvedRequests = await _borrowRequestRepository.GetBorrowRequestsByUserIdAndStatusAsync(userId, Enums.BorrowRequestStatus.Approved);

            return approvedRequests.Select(r => new UserApprovedRequestDto
            {
                RequestId = r.RequestId,
                InstanceCode = r.InstanceCode,
                DeviceName = r.DeviceName,
                StorageLocation = r.StorageLocation
            });
        }

        public async Task<IEnumerable<UserRejectedRequestDto>> GetUserRejectedRequestAsync(int userId)
        {
            var rejectedRequests = await _borrowRequestRepository.GetBorrowRequestsByUserIdAndStatusAsync(userId, Enums.BorrowRequestStatus.Rejected);

            return rejectedRequests.Select(r => new UserRejectedRequestDto
            {
                RequestId = r.RequestId,
                DeviceName = r.DeviceName,
                RequestDate = r.RequestDate,
                StartPeriod = r.StartPeriod,
                EndPeriod = r.EndPeriod,
                UsageLocation = r.UsageLocation,
                Purpose = r.Purpose
            });
        }
    }
}
