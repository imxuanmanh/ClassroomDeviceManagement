using ClassroomDeviceManagement.Managers;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Repositories.Interfaces;

namespace ClassroomDeviceManagement.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly SseConnectionManager _sseManager;

        public NotificationService(INotificationRepository notificationRepository, SseConnectionManager sseManager)
        {
            _notificationRepository = notificationRepository;
            _sseManager = sseManager;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
        }

        public async Task PushNotificationAsync(int userId, string content)
        {
            // 1. Lưu vào database
            var notification = await _notificationRepository.AddNotificationAsync(userId, content);

            if (notification != null)
            {
                // 2. Push real-time nếu user đang online
                await _sseManager.SendNotificationAsync(userId, new
                {
                    id = notification.Id,
                    userId = notification.UserId,
                    content = notification.Content,
                    seen = notification.Seen,
                    deleted = notification.Deleted
                });
            }
        }
        public async Task<bool> MarkAsSeenAsync(int notificationId)
        {
            return await _notificationRepository.ChangeNotificationStatusAsync(notificationId: notificationId, isSeen: true, isDeleted: false);
        }

        public async Task<bool> MarkAsDeletedAsync(int notificationId)
        {
            return await _notificationRepository.ChangeNotificationStatusAsync(notificationId: notificationId, isSeen: false, isDeleted: true);
        }

        public async Task SubscribeAsync(int userId, HttpResponse response, CancellationToken cancellationToken)
        {
            // 1. Cấu hình Header cho SSE 
            response.Headers.Append("Content-Type", "text/event-stream");
            response.Headers.Append("Cache-Control", "no-cache");
            response.Headers.Append("Connection", "keep-alive");

            // 2. Thêm vào Manager
            _sseManager.AddConnection(userId, response);

            try
            {
                // 3. Giữ kết nối (Blocking)
                // Hàm này sẽ treo ở đây cho đến khi Client ngắt kết nối
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Client ngắt kết nối, không cần log lỗi
            }
            finally
            {
                // 4. Dọn dẹp
                _sseManager.RemoveConnection(userId);
            }
        }
        public async Task<bool> MarkAllAsSeenAsync(int userId)
        {
            return await _notificationRepository.ChangeAllNotificationStatusAsync(userId: userId, isSeen: true, isDeleted: false);
        }

        public async Task<bool> MarkAllAsDeletedAsync(int userId)
        {
            return await _notificationRepository.ChangeAllNotificationStatusAsync(userId: userId, isSeen: false, isDeleted: true);
        }
    }

}