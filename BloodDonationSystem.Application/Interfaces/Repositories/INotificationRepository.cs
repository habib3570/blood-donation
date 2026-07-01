using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetByUserIdAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task<List<Notification>> GetPinnedNotificationsAsync(string userId);
        Task<bool> IsUserMutedAsync(string userId);

        // ✅ নতুন তিনটা method যোগ করা হলো
        Task AddMuteAsync(NotificationMute mute);
        Task RemoveMuteAsync(string userId);
        Task PinNotificationAsync(int notificationId);
    }
}