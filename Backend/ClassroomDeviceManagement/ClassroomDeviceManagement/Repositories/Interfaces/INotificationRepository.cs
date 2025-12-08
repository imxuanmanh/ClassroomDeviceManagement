using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<Notification?> AddNotificationAsync(int userId, string content);
        Task<bool> MarkAsSeenAsync(int notificationId);
        Task<bool> ChangeNotificationStatusAsync(int notificationId, bool isSeen, bool isDeleted);
        Task<bool> ChangeAllNotificationStatusAsync(int userId, bool isSeen, bool isDeleted);
    }
}