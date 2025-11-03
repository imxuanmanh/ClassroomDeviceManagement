using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Enums;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.ViewModels.BorrowRequest;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class BorrowRequestRepository : IBorrowRequestRepository
    {
        private readonly IDbManager _dbManager;

        public BorrowRequestRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<IEnumerable<PendingRequestViewModel>> GetPendingBorrowRequestsAsync()
        {
            var results = new List<PendingRequestViewModel>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    br.request_id AS RequestId,
                    u.fullname AS Borrower,
                    dm.model_name AS DeviceName,
                    di.instance_code AS InstanceCode,
                    br.request_date AS RequestDate,
                    br.usage_location AS UsageLocation,
                    br.purpose AS Purpose
                FROM 
                    borrow_request br
                JOIN 
                    device_instance di ON di.instance_id = br.instance_id
                JOIN 
                    device_model dm ON dm.model_id = di.model_id
                JOIN
                    [user] u ON u.user_id = br.user_id
                WHERE
                    br.status_id = @status;
                ",

                async reader =>
                {
                    int requestIdIndex = reader.GetOrdinal("RequestId");
                    int borrowerIndex = reader.GetOrdinal("Borrower");
                    int deviceNameIndex = reader.GetOrdinal("DeviceName");
                    int instanceCodeIndex = reader.GetOrdinal("InstanceCode");
                    int requestDateIndex = reader.GetOrdinal("RequestDate");
                    int usageLocationIndex = reader.GetOrdinal("UsageLocation");
                    int purposeIndex = reader.GetOrdinal("Purpose");

                    while (await reader.ReadAsync())
                    {
                        results.Add(new PendingRequestViewModel
                        {
                            RequestId = reader.GetInt32(requestIdIndex),
                            Borrower = reader.GetString(borrowerIndex),
                            DeviceName = reader.GetString(deviceNameIndex),
                            InstanceCode = reader.GetString(instanceCodeIndex),
                            RequestDate = reader.GetDateTime(requestDateIndex),
                            UsageLocation = reader.GetString(usageLocationIndex),
                            Purpose = reader.GetString(purposeIndex)
                        });
                    }
                },
                new SqlParameter("status", (int)BorrowRequestStatus.Pending));
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }

            return results;
        }
        public async Task<IEnumerable<ApprovedRequestViewModel>> GetApprovedBorrowRequestsAsync()
        {
            var results = new List<ApprovedRequestViewModel>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    br.request_id AS RequestId,
                    u.fullname AS Borrower,
                    dm.model_name AS DeviceName,
                    di.instance_code AS InstanceCode,
                    br.request_date AS RequestDate,
                    br.approved_date AS ApprovedDate,
                    br.usage_location AS UsageLocation
                FROM 
                    borrow_request br
                JOIN 
                    device_instance di ON di.instance_id = br.instance_id
                JOIN 
                    device_model dm ON dm.model_id = di.model_id
                JOIN
                    [user] u ON u.user_id = br.user_id
                WHERE
                    br.status_id = @status;
                ",

                async reader =>
                {
                    int requestIdIndex = reader.GetOrdinal("RequestId");
                    int borrowerIndex = reader.GetOrdinal("Borrower");
                    int deviceNameIndex = reader.GetOrdinal("DeviceName");
                    int instanceCodeIndex = reader.GetOrdinal("InstanceCode");
                    int requestDateIndex = reader.GetOrdinal("RequestDate");
                    int approvedDateIndex = reader.GetOrdinal("ApprovedDate");
                    int usageLocationIndex = reader.GetOrdinal("UsageLocation");

                    while (await reader.ReadAsync())
                    {
                        results.Add(new ApprovedRequestViewModel
                        {
                            RequestId = reader.GetInt32(requestIdIndex),
                            Borrower = reader.GetString(borrowerIndex),
                            DeviceName = reader.GetString(deviceNameIndex),
                            InstanceCode = reader.GetString(instanceCodeIndex),
                            RequestDate = reader.GetDateTime(requestDateIndex),
                            ApprovedDate = reader.GetDateTime(approvedDateIndex),
                            UsageLocation = reader.GetString(usageLocationIndex),
                        });
                    }
                },
                new SqlParameter("status", (int)BorrowRequestStatus.Approved));
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }
            return results;
        }

        public async Task<IEnumerable<RejectedRequestViewModel>> GetRejectedBorrowRequestsAsync()
        {
            var results = new List<RejectedRequestViewModel>();
            
            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    br.request_id AS RequestId,
                    u.fullname AS Borrower,
                    dm.model_name AS DeviceName,
                    di.instance_code AS InstanceCode,
                    br.request_date AS RequestDate,
                    br.approved_date AS ApprovedDate,
                    br.usage_location AS UsageLocation
                FROM 
                    borrow_request br
                JOIN 
                    device_instance di ON di.instance_id = br.instance_id
                JOIN 
                    device_model dm ON dm.model_id = di.model_id
                JOIN
                    [user] u ON u.user_id = br.user_id
                WHERE
                    br.status_id = @status;
                ",

                async reader =>
                {
                    int requestIdIndex = reader.GetOrdinal("RequestId");
                    int borrowerIndex = reader.GetOrdinal("Borrower");
                    int deviceNameIndex = reader.GetOrdinal("DeviceName");
                    int instanceCodeIndex = reader.GetOrdinal("InstanceCode");
                    int requestDateIndex = reader.GetOrdinal("RequestDate");

                    while (await reader.ReadAsync())
                    {
                        results.Add(new RejectedRequestViewModel
                        {
                            RequestId = reader.GetInt32(requestIdIndex),
                            Borrower = reader.GetString(borrowerIndex),
                            DeviceName = reader.GetString(deviceNameIndex),
                            InstanceCode = reader.GetString(instanceCodeIndex),
                            RequestDate = reader.GetDateTime(requestDateIndex)
                        });
                    }
                },
                new SqlParameter("status", (int)BorrowRequestStatus.Rejected));
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }

            return results;

        }
        public async Task<IEnumerable<ReturnedRequestViewModel>> GetReturnedBorrowRequestsAsync()
        {
            var results = new List<ReturnedRequestViewModel>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT 
                    br.request_id AS RequestId,
                    u.fullname AS Borrower,
                    dm.model_name AS DeviceName,
                    di.instance_code AS InstanceCode,
                    br.request_date AS RequestDate,
                    br.return_date AS ReturnDate
                FROM 
                    borrow_request br
                JOIN 
                    device_instance di ON di.instance_id = br.instance_id
                JOIN 
                    device_model dm ON dm.model_id = di.model_id
                JOIN
                    [user] u ON u.user_id = br.user_id
                WHERE
                    br.status_id = @status;
                ",

                async reader =>
                {
                    int requestIdIndex = reader.GetOrdinal("RequestId");
                    int borrowerIndex = reader.GetOrdinal("Borrower");
                    int deviceNameIndex = reader.GetOrdinal("DeviceName");
                    int instanceCodeIndex = reader.GetOrdinal("InstanceCode");
                    int requestDateIndex = reader.GetOrdinal("RequestDate");
                    int returnedDateIndex = reader.GetOrdinal("ReturnDate");

                    while (await reader.ReadAsync())
                    {
                        results.Add(new ReturnedRequestViewModel
                        {
                            RequestId = reader.GetInt32(requestIdIndex),
                            Borrower = reader.GetString(borrowerIndex),
                            DeviceName = reader.GetString(deviceNameIndex),
                            InstanceCode = reader.GetString(instanceCodeIndex),
                            RequestDate = reader.GetDateTime(requestDateIndex),
                            ReturnDate = reader.GetDateTime(returnedDateIndex)
                        });
                    }
                },
                new SqlParameter("status", (int)BorrowRequestStatus.Returned));
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }

            return results;
        }

        public async Task<BorrowRequestDto?> AddBorrowRequestAsync(BorrowRequest request)
        {
            BorrowRequestDto? result = null;

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                DECLARE @RequestId INT;

                INSERT INTO 
                    borrow_request (user_id, instance_id, request_date, usage_location, status_id, purpose)
                    
                VALUES (@userId, @instanceId, @requestDate, @usageLocation, @statusId, @purpose);
                
                SET @RequestId = SCOPE_IDENTITY();
                
                SELECT
                    br.request_id AS RequestId,  
                    u.fullname AS Borrower,
                	di.instance_code AS InstanceCode,
                	dm.model_name AS DeviceName,
                	br.request_date AS RequestDate,
                	br.usage_location AS UsageLocation,
                    br.purpose AS Purpose
                
                FROM
                    borrow_request br
                
                JOIN [user] u ON br.user_id = u.user_id
                
                JOIN device_instance di ON br.instance_id = di.instance_id
                
                JOIN device_model dm ON dm.model_id = di.model_id
                
                WHERE 
                	br.request_id = @RequestId;
                ",

                async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        result = new BorrowRequestDto();

                        result.Borrower = reader.GetString(reader.GetOrdinal("Borrower"));
                        result.InstanceCode = reader.GetString(reader.GetOrdinal("InstanceCode"));
                        result.DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"));
                        result.RequestDate = reader.GetDateTime(reader.GetOrdinal("RequestDate"));
                        result.UsageLocation = reader.GetString(reader.GetOrdinal("UsageLocation"));
                        result.Purpose = reader.GetString(reader.GetOrdinal("Purpose"));
                    }
                },

                new SqlParameter("@userId", request.UserId),
                new SqlParameter("@instanceId", request.InstanceId),
                new SqlParameter("@requestDate", request.RequestDate),
                new SqlParameter("@usageLocation", request.UsageLocation),
                new SqlParameter("@statusId", request.Status),
                new SqlParameter("@purpose", request.Purpose)
                );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
            }

            return result;
        }
    
        public async Task<bool> UpdateBorrowRequestStatusAsync(int requestId, BorrowRequestStatus newStatus, DateTime? approvedDate = null, DateTime? returnDate = null)
        {
            try
            {
                int row = await _dbManager.ExecuteNonQueryAsync(
                @"
                UPDATE 
                    borrow_request
                SET 
                    status_id = @statusId,
                    approved_date = CASE WHEN @approvedDate IS NOT NULL THEN @approvedDate ELSE approved_date END,
                    return_date = CASE WHEN @returnDate IS NOT NULL THEN @returnDate ELSE return_date END
                WHERE 
                    request_id = @requestId;
                ",
                new SqlParameter("@statusId", (int)newStatus),
                new SqlParameter("@approvedDate", (object?)approvedDate ?? DBNull.Value),
                new SqlParameter("@returnDate", (object?)returnDate ?? DBNull.Value),
                new SqlParameter("@requestId", requestId)
                );

                return row > 0;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }  
        }

        public async Task<int?> GetInstanceIdByRequestIdAsync(int requestId)
        {
            int? instanceId = await _dbManager.ExecuteScalarAsync<int>(
                @"
                SELECT 
                    instance_id 
                FROM 
                    borrow_request 
                WHERE 
                    request_id = @requestId
                ",
                new SqlParameter("requestId", requestId)
                );
            return instanceId;
        }
        
    }
}
