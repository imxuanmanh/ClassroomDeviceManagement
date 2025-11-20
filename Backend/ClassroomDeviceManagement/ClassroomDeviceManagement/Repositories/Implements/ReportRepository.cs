using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Enums;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.ViewModels.Report;
using ClassroomDeviceManagement.Dto;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbManager _dbManager;

        public ReportRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        //public async Task<bool> AddReportAsync(Report report)
        //{
        //    ReportDto? result = null;

        //    try
        //    {
        //        await _dbManager.ExecuteQueryAsync(
        //        @"
        //        DECLARE @ReportId INT;

        //        INSERT INTO report (user_id, instance_id, description, image_path, report_date, status_id)
        //        VALUES (@userId, @instanceId, @description, @imagePath, @reportDate, @statusId);

        //        SET @ReportId = SCOPE_IDENTITY();

        //        SELECT
        //            r.report_id AS ReportId,
        //            u.fullname AS Reporter,
        //            di.instance_code AS InstanceCode,
        //            dm.model_name AS DeviceName,
        //            r.description AS Description,
        //            r.image_path AS ImagePath,
        //            r.report_date AS ReportDate,
        //            rs.name AS Status
        //        FROM report r
        //        JOIN [user] u ON r.user_id = u.user_id
        //        JOIN device_instance di ON r.instance_id = di.instance_id
        //        JOIN device_model dm ON dm.model_id = di.model_id
        //        JOIN report_status rs ON r.status_id = rs.id
        //        WHERE r.report_id = @ReportId;
        //        ",
        //        async reader =>
        //        {
        //            if (await reader.ReadAsync())
        //            {
        //                result = new ReportDto
        //                {
        //                    ReportId = reader.GetInt32(reader.GetOrdinal("ReportId")),
        //                    InstanceCode = reader.GetString(reader.GetOrdinal("InstanceCode")),
        //                    DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
        //                    Description = reader.GetString(reader.GetOrdinal("Description")),
        //                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImagePath")),
        //                    ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate")),
        //                    Status = reader.GetString(reader.GetOrdinal("Status"))
        //                };
        //            }
        //        },
        //        new SqlParameter("@userId", report.UserId),
        //        new SqlParameter("@instanceId", report.InstanceId),
        //        new SqlParameter("@description", report.Description),
        //        new SqlParameter("@imagePath", (object?)report.ImagePath),
        //        new SqlParameter("@reportDate", report.ReportDate),
        //        new SqlParameter("@statusId", (int)ReportStatus.New)
        //        );
        //    }
        //    catch (SqlException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
        //    }

        //    return result;
        //}

        public async Task<bool> AddReportAsync(Report report)
        {
            int newId = 0;

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                    INSERT INTO report (user_id, instance_id, description, image_path, report_date, status_id)
                    VALUES (@userId, @instanceId, @description, @imagePath, @reportDate, @statusId);

                    SELECT SCOPE_IDENTITY() AS NewId;
                ",
                async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        newId = Convert.ToInt32(reader["NewId"]);
                    }
                },
                new SqlParameter("@userId", report.UserId),
                new SqlParameter("@instanceId", report.InstanceId),
                new SqlParameter("@description", report.Description),
                new SqlParameter("@imagePath", (object?)report.ImagePath ?? DBNull.Value),
                new SqlParameter("@reportDate", report.ReportDate),
                new SqlParameter("@statusId", (int)ReportStatus.New)
                );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }

            return newId > 0;
        }


        public async Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(int statusId)
        {
            var results = new List<ReportDto>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT
                    r.report_id AS ReportId,
                    u.fullname AS Reporter,
                    di.instance_code AS InstanceCode,
                    dm.model_name AS DeviceName,
                    r.description AS Description,
                    r.image_path AS ImagePath,
                    r.report_date AS ReportDate,
                    rs.name AS Status
                FROM report r
                JOIN [user] u ON r.user_id = u.user_id
                JOIN device_instance di ON r.instance_id = di.instance_id
                JOIN device_model dm ON dm.model_id = di.model_id
                JOIN report_status rs ON r.status_id = rs.id
                WHERE r.status_id = @statusId;
                ",
                async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(new ReportDto
                        {
                            ReportId = reader.GetInt32(reader.GetOrdinal("ReportId")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("InstanceCode")),
                            DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImagePath")),
                            ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        });
                    }
                },
                new SqlParameter("@statusId", statusId)
                );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }

            return results;
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, int newStatusId)
        {
            try
            {
                int row = await _dbManager.ExecuteNonQueryAsync(
                @"
                UPDATE report
                SET status_id = @statusId
                WHERE report_id = @reportId;
                ",
                new SqlParameter("@statusId", newStatusId),
                new SqlParameter("@reportId", reportId)
                );

                return row > 0;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
        }
    }
}
