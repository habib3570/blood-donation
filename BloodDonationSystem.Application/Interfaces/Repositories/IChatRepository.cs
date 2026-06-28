using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IChatRepository : IGenericRepository<ChatMessage>
    {
        Task<List<ChatMessage>> GetConversationAsync(string userId1, string userId2, int page = 1, int pageSize = 20);
        Task<List<ChatMessage>> GetUnreadMessagesAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(string senderId, string receiverId);
        Task<List<string>> GetChatContactsAsync(string userId);
        Task<ChatMessage?> GetLastMessageAsync(string userId1, string userId2);
    }
}