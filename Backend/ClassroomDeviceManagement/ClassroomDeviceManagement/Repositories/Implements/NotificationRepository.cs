using ClassroomDeviceManagement.Models;
using Microsoft.Data.SqlClient;
using ClassroomDeviceManagement.Repositories.Interfaces;
using ClassroomDeviceManagement.Managers;
using System.Data;

namespace ClassroomDeviceManagement.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbManager _dbManager;

        public NotificationRepository(IDbManager dbManager)
        {
            _dbManager = dbManager;
        }
        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            List<Notification> notifications = new List<Notification>();

            try
            {
                await _dbManager.ExecuteQueryAsync(
                    @"SELECT
                        id AS Id,
                        user_id AS UserId,
                        content AS Content,
                        seen AS Seen,
                        deleted AS Deleted

                    FROM 
                        notifications

                    WHERE
                        user_id = @userId
                        
                    AND 
                        deleted = 0;"
                    ,
                    async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            int idIndex = reader.GetOrdinal("Id");
                            int userIdIndex = reader.GetOrdinal("UserId");
                            int contentIndex = reader.GetOrdinal("Content");
                            int seenId = reader.GetOrdinal("Seen");
                            int deletedIndex = reader.GetOrdinal("Deleted");
                            notifications.Add(new Notification
                            {
                                Id = reader.GetInt32(idIndex),
                                UserId = reader.GetInt32(userIdIndex),
                                Content = reader.GetString(contentIndex),
                                Seen = reader.GetBoolean(seenId),
                                Deleted = reader.GetBoolean(deletedIndex)
                            });
                        }
                    },
                    new SqlParameter("@userId", userId)
                );
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return notifications;
        }

        public async Task<Notification?> AddNotificationAsync(int userId, string content)
        {
            Notification? notification = null;

            try
            {
                await _dbManager.ExecuteQueryAsync(
                    @"INSERT INTO notifications (user_id, content, seen, deleted)
              OUTPUT INSERTED.id, INSERTED.user_id, INSERTED.content, 
                     INSERTED.seen, INSERTED.deleted
              VALUES (@userId, @content, 0, 0);",
                    async reader =>
                    {
                        if (await reader.ReadAsync())
                        {
                            notification = new Notification
                            {
                                Id = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Content = reader.GetString(2),
                                Seen = reader.GetBoolean(3),
                                Deleted = reader.GetBoolean(4)
                            };
                        }
                    },
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@content", content)
                );
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error creating notification: {ex.Message}");
            }

            return notification;
        }
        public async Task<bool> MarkAsSeenAsync(int notificationId)
        {
            try
            {
                int rowsAffected = await _dbManager.ExecuteNonQueryAsync(
                    @"
                        UPDATE 
                            notifications

                        SET 
                            seen = 1 
                        
                        WHERE id = @notificationId;
                    ",
                    new SqlParameter("@notificationId", notificationId)
                );

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error marking notification as seen: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> ChangeNotificationStatusAsync(int notificationId, bool isSeen = false, bool isDeleted = false)
        {
            try
            {
                int rowsAffected = await _dbManager.ExecuteNonQueryAsync(
                    @"
                        UPDATE 
                            notifications

                        SET 
                            seen = @isSeen, 
                            deleted = @isDeleted
                        
                        WHERE id = @notificationId;
                    ",
                    new SqlParameter("@isSeen", isSeen),
                    new SqlParameter("@isDeleted", isDeleted),
                    new SqlParameter("@notificationId", notificationId)
                );

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error marking notification as seen: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeAllNotificationStatusAsync(int userId, bool isSeen, bool isDeleted)
        {
            try
            {
                int rowsAffected = await _dbManager.ExecuteNonQueryAsync(
                    @"
                        UPDATE 
                            notifications

                        SET 
                            seen = @isSeen, 
                            deleted = @isDeleted
                        
                        WHERE user_id = @userId;
                    ",
                    new SqlParameter("@isSeen", isSeen),
                    new SqlParameter("@isDeleted", isDeleted),
                    new SqlParameter("@userId", userId)
                );

                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error marking notification as seen: {ex.Message}");
                return false;
            }
        }

    }
}
