using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly Dictionary<string, string> _connectedUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                _connectedUsers[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"notify_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
                _connectedUsers.Remove(userId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinEmergencyGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "emergency_alerts");
        }

        public static async Task SendNotificationToUser(IHubContext<NotificationHub> hubContext, string userId, object notification)
        {
            await hubContext.Clients.Group($"notify_{userId}").SendAsync("ReceiveNotification", notification);
        }

        public static async Task SendEmergencyAlert(IHubContext<NotificationHub> hubContext, object alert)
        {
            await hubContext.Clients.Group("emergency_alerts").SendAsync("EmergencyAlert", alert);
        }
    }
}