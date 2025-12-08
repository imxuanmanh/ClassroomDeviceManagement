using ClassroomDeviceManagement.Models;

namespace ClassroomDeviceManagement.Services.Implements
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task PushNotificationAsync(int userId, string content);
        Task<bool> MarkAsSeenAsync(int notificationId);
        Task<bool> MarkAsDeletedAsync(int notificationId);
        Task<bool> MarkAllAsSeenAsync(int userId);
        Task<bool> MarkAllAsDeletedAsync(int userId);
        Task SubscribeAsync(int userId, HttpResponse response, CancellationToken cancellationToken);

    }
}