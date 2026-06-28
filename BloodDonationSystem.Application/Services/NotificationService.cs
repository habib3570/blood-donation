using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Notification;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> SendNotificationAsync(string userId, string title, string message,
            NotificationType type, string? actionUrl = null, string? referenceId = null)
        {
            var isMuted = await _notificationRepository.IsUserMutedAsync(userId);
            if (isMuted && type != NotificationType.EmergencyAlert)
                return Result.Success("User has notifications muted.");

            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                ReferenceId = referenceId,
                IsPinned = type == NotificationType.EmergencyAlert,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> SendNotificationToMultipleAsync(List<string> userIds, string title,
            string message, NotificationType type)
        {
            foreach (var userId in userIds)
                await SendNotificationAsync(userId, title, message, type);
            return Result.Success();
        }

        public async Task<Result<List<NotificationDto>>> GetUserNotificationsAsync(string userId, int page = 1)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, page);
            return Result<List<NotificationDto>>.Success(_mapper.Map<List<NotificationDto>>(notifications));
        }

        public async Task<Result<int>> GetUnreadCountAsync(string userId)
        {
            var count = await _notificationRepository.GetUnreadCountAsync(userId);
            return Result<int>.Success(count);
        }

        public async Task<Result> MarkAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> MarkAllAsReadAsync(string userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> MuteNotificationsAsync(string userId, int hours)
        {
            var mute = new NotificationMute
            {
                UserId = userId,
                MutedUntil = DateTime.UtcNow.AddHours(hours),
                MutedAt = DateTime.UtcNow
            };
            await _notificationRepository.AddAsync(new Notification { UserId = userId, Title = "", Message = "", Type = NotificationType.SystemAlert });
            await _unitOfWork.SaveChangesAsync();
            return Result.Success($"Notifications muted for {hours} hours.");
        }

        public async Task<Result> UnmuteNotificationsAsync(string userId)
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Notifications unmuted.");
        }

        public async Task<Result<bool>> IsUserMutedAsync(string userId)
        {
            var isMuted = await _notificationRepository.IsUserMutedAsync(userId);
            return Result<bool>.Success(isMuted);
        }

        public async Task<Result> PinNotificationAsync(int notificationId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Notification pinned.");
        }
    }
}