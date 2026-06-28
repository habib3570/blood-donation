using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private static readonly Dictionary<string, string> _connectedUsers = new();

        public ChatHub(IChatService chatService, INotificationService notificationService)
        {
            _chatService = chatService;
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                _connectedUsers[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
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

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;

            var result = await _chatService.SendMessageAsync(senderId, new Application.DTOs.Chat.SendMessageDto
            {
                ReceiverId = receiverId,
                Message = message
            });

            if (result.IsSuccess && result.Data != null)
            {
                await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", result.Data);
                await Clients.Group($"user_{senderId}").SendAsync("MessageSent", result.Data);
            }
        }

        public async Task MarkAsRead(string senderId)
        {
            var receiverId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receiverId == null) return;
            await _chatService.MarkAsReadAsync(senderId, receiverId);
        }

        public async Task StartTyping(string receiverId)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;
            await Clients.Group($"user_{receiverId}").SendAsync("UserTyping", senderId);
        }

        public async Task StopTyping(string receiverId)
        {
            var senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;
            await Clients.Group($"user_{receiverId}").SendAsync("UserStoppedTyping", senderId);
        }

        public static bool IsUserOnline(string userId) => _connectedUsers.ContainsKey(userId);
    }
}