using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Notification>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 20)
            => await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
            => await _dbSet
                .Where(x => x.UserId == userId && !x.IsRead)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(string userId)
            => await _dbSet.CountAsync(x => x.UserId == userId && !x.IsRead);

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _dbSet.Update(notification);
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _dbSet
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
        }

        public async Task<List<Notification>> GetPinnedNotificationsAsync(string userId)
            => await _dbSet
                .Where(x => x.UserId == userId && x.IsPinned)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<bool> IsUserMutedAsync(string userId)
        {
            var mute = await _context.NotificationMutes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.MutedUntil > DateTime.UtcNow);
            return mute != null;
        }
    }
}