using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Chat;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<ChatMessageDto>> SendMessageAsync(string senderId, SendMessageDto dto);
        Task<Result<List<ChatMessageDto>>> GetConversationAsync(string userId1, string userId2, int page = 1);
        Task<Result<int>> GetUnreadCountAsync(string userId);
        Task<Result> MarkAsReadAsync(string senderId, string receiverId);
        Task<Result<List<string>>> GetChatContactsAsync(string userId);
        Task<Result<ChatMessageDto?>> GetLastMessageAsync(string userId1, string userId2);
    }
}