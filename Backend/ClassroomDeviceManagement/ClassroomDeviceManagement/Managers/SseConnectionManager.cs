using System.Collections.Concurrent;
using System.Text.Json;

namespace ClassroomDeviceManagement.Managers
{
    public class SseConnectionManager
    {
        private readonly ConcurrentDictionary<int, HttpResponse> _connections = new();

        public void AddConnection(int userId, HttpResponse response)
        {
            _connections[userId] = response;
            Console.WriteLine($"User {userId} connected. Total connections: {_connections.Count}");
        }

        public void RemoveConnection(int userId)
        {
            _connections.TryRemove(userId, out _);
            Console.WriteLine($"User {userId} disconnected. Total connections: {_connections.Count}");
        }

        public async Task SendNotificationAsync(int userId, object notification)
        {
            if (_connections.TryGetValue(userId, out var response))
            {
                try
                {
                    var json = JsonSerializer.Serialize(notification);
                    await response.WriteAsync($"data: {json}\n\n");
                    await response.Body.FlushAsync();
                    Console.WriteLine($"Sent notification to user {userId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending to user {userId}: {ex.Message}");
                    RemoveConnection(userId);
                }
            }
            else
            {
                Console.WriteLine($"User {userId} not connected");
            }
        }

        public bool IsUserConnected(int userId)
        {
            return _connections.ContainsKey(userId);
        }
    }
}