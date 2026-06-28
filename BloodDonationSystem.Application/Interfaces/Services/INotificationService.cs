using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Notification;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<Result> SendNotificationAsync(string userId, string title, string message, NotificationType type, string? actionUrl = null, string? referenceId = null);
        Task<Result> SendNotificationToMultipleAsync(List<string> userIds, string title, string message, NotificationType type);
        Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(string userId, int page = 1);
        Task<Result<int>> GetUnreadCountAsync(string userId);
        Task<Result> MarkAsReadAsync(int notificationId);
        Task<Result> MarkAllAsReadAsync(string userId);
        Task<Result> MuteNotificationsAsync(string userId, int hours);
        Task<Result> UnmuteNotificationsAsync(string userId);
        Task<Result<bool>> IsUserMutedAsync(string userId);
        Task<Result> PinNotificationAsync(int notificationId);
    }
}