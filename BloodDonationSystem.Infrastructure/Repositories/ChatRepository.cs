using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class ChatRepository : GenericRepository<ChatMessage>, IChatRepository
    {
        public ChatRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<ChatMessage>> GetConversationAsync(string userId1, string userId2, int page = 1, int pageSize = 20)
            => await _dbSet
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .Where(x => (x.SenderId == userId1 && x.ReceiverId == userId2) ||
                            (x.SenderId == userId2 && x.ReceiverId == userId1))
                .OrderByDescending(x => x.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.SentAt)
                .ToListAsync();

        public async Task<List<ChatMessage>> GetUnreadMessagesAsync(string userId)
            => await _dbSet
                .Include(x => x.Sender)
                .Where(x => x.ReceiverId == userId && !x.IsRead)
                .OrderBy(x => x.SentAt)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _dbSet.CountAsync(x => x.ReceiverId == userId && !x.IsRead);

        public async Task MarkAsReadAsync(string senderId, string receiverId)
        {
            var messages = await _dbSet
                .Where(x => x.SenderId == senderId && x.ReceiverId == receiverId && !x.IsRead)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = true;
                msg.ReadAt = DateTime.UtcNow;
            }
        }

        public async Task<List<string>> GetChatContactsAsync(string userId)
        {
            var sent = await _dbSet.Where(x => x.SenderId == userId).Select(x => x.ReceiverId).Distinct().ToListAsync();
            var received = await _dbSet.Where(x => x.ReceiverId == userId).Select(x => x.SenderId).Distinct().ToListAsync();
            return sent.Union(received).Distinct().ToList();
        }

        public async Task<ChatMessage?> GetLastMessageAsync(string userId1, string userId2)
            => await _dbSet
                .Where(x => (x.SenderId == userId1 && x.ReceiverId == userId2) ||
                            (x.SenderId == userId2 && x.ReceiverId == userId1))
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefaultAsync();
    }
}