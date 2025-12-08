using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Enums;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.ViewModels.Report;
using ClassroomDeviceManagement.Dto;
using System.Net.Http.Headers;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbManager _dbManager;

        public ReportRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }

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
        public async Task<Report?> GetByIdAsync(int reportId)
        {
            Report? result = null;

            await _dbManager.ExecuteQueryAsync(
                "SELECT * FROM report WHERE report_id = @reportId",
                async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        result = new Report
                        {
                            ReportId = reader.GetInt32(reader.GetOrdinal("report_id")),
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            InstanceId = reader.GetInt32(reader.GetOrdinal("instance_id")),
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path")) ? null : reader.GetString(reader.GetOrdinal("image_path")),
                            ReportDate = reader.GetDateTime(reader.GetOrdinal("report_date")),
                            Status = (ReportStatus)reader.GetInt32(reader.GetOrdinal("status_id"))
                        };
                    }
                },
                new SqlParameter("@reportId", reportId)
            );

            return result;
        }


        public async Task<IEnumerable<ReportDto>> GetReportsByStatusAsync(ReportStatus status)
        {
            var results = new List<ReportDto>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                @"
                SELECT
                    r.report_id AS ReportId,
                    u.user_id AS ReporterId,
                    u.fullname AS Reporter,
                    di.instance_id AS InstanceId,
                    di.instance_code AS InstanceCode,
                    dm.model_name AS DeviceName,
                    r.description AS Description,
                    ISNULL(r.image_path, '') AS ImagePath,
                    r.report_date AS ReportDate,
                    rs.id AS StatusId
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
                            UserId = reader.GetInt32(reader.GetOrdinal("ReporterId")),
                            UserFullName = reader.GetString(reader.GetOrdinal("Reporter")),
                            InstanceId = reader.GetInt32(reader.GetOrdinal("InstanceId")),
                            InstanceCode = reader.GetString(reader.GetOrdinal("InstanceCode")),
                            DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? string.Empty : reader.GetString(reader.GetOrdinal("ImagePath")),
                            ReportDate = reader.GetDateTime(reader.GetOrdinal("ReportDate")),
                            Status = (ReportStatus)reader.GetInt32(reader.GetOrdinal("StatusId"))
                        });
                    }
                },
                new SqlParameter("@statusId", (int)status)
                );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }

            return results;
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus newStatus)
        {
            try
            {
                int row = await _dbManager.ExecuteNonQueryAsync(
                @"
                UPDATE report
                SET status_id = @statusId
                WHERE report_id = @reportId;
                ",
                new SqlParameter("@statusId", (int)newStatus),
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
