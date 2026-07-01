using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Notification;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace BloodDonationSystem.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IEmergencyRequestRepository _emergencyRequestRepository;
        private readonly IDonorRepository _donorRepository;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBloodRequestRepository bloodRequestRepository,
            IEmergencyRequestRepository emergencyRequestRepository,
            IDonorRepository donorRepository)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bloodRequestRepository = bloodRequestRepository;
            _emergencyRequestRepository = emergencyRequestRepository;
            _donorRepository = donorRepository;
        }
        public async Task<Result> SendNotificationAsync(string userId, string title, string message,
    Domain.Enums.NotificationType type, string? actionUrl = null, string? referenceId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                IsRead = false,
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

       

        public async Task<Result<bool>> IsUserMutedAsync(string userId)
        {
            var isMuted = await _notificationRepository.IsUserMutedAsync(userId);
            return Result<bool>.Success(isMuted);
        }

      
        public async Task<Result> MuteNotificationsAsync(string userId, int hours)
        {
            var mute = new NotificationMute
            {
                UserId = userId,
                MutedUntil = DateTime.UtcNow.AddHours(hours),
                MutedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddMuteAsync(mute); 
            await _unitOfWork.SaveChangesAsync();
            return Result.Success($"Notifications muted for {hours} hours.");
        }

        
        public async Task<Result> UnmuteNotificationsAsync(string userId)
        {
            await _notificationRepository.RemoveMuteAsync(userId); 
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Notifications unmuted.");
        }


        public async Task<Result> PinNotificationAsync(int notificationId)
        {
            await _notificationRepository.PinNotificationAsync(notificationId); 
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Notification pinned.");
        }
        public async Task NotifyMatchingDonorsForRequestAsync(int bloodRequestId)
        {
            var request = await _bloodRequestRepository.GetByIdAsync(bloodRequestId);
            if (request == null) return;

            var matchingDonors = await _donorRepository.GetDonorsByBloodGroupAsync(request.BloodGroup);
            var bgDisplay = GetBloodGroupDisplay(request.BloodGroup);

            foreach (var donor in matchingDonors)
            {
                if (donor.UserId == request.RequesterId) continue;

                await SendNotificationAsync(
                    donor.UserId,
                    $"🩸 {bgDisplay} Blood Needed!",
                    $"{request.PatientName} needs {bgDisplay} blood at {request.HospitalName}, {request.District}. Can you help?",
                    Domain.Enums.NotificationType.NewBloodRequest,
                    actionUrl: $"/BloodRequest/Details/{bloodRequestId}");
            }
        }

        public async Task NotifyMatchingDonorsForEmergencyAsync(int emergencyRequestId)
        {
            var emergency = await _emergencyRequestRepository.GetByIdAsync(emergencyRequestId);
            if (emergency == null) return;

            var matchingDonors = await _donorRepository.GetDonorsByBloodGroupAsync(emergency.BloodGroup);
            var bgDisplay = GetBloodGroupDisplay(emergency.BloodGroup);

            foreach (var donor in matchingDonors)
            {
                if (donor.UserId == emergency.RequesterId) continue;

                await SendNotificationAsync(
                    donor.UserId,
                    $"🚨 URGENT: {bgDisplay} Blood Needed!",
                    $"EMERGENCY: {emergency.PatientName} urgently needs {bgDisplay} blood at {emergency.HospitalName}. Please respond immediately!",
                    Domain.Enums.NotificationType.EmergencyAlert,
                    actionUrl: $"/Emergency/Details/{emergencyRequestId}");
            }
        }

        public async Task NotifyDonorsRequestFulfilledAsync(int bloodRequestId, string acceptedDonorName)
        {
            var request = await _bloodRequestRepository.GetByIdAsync(bloodRequestId);
            if (request == null) return;

            var matchingDonors = await _donorRepository.GetDonorsByBloodGroupAsync(request.BloodGroup);
            var bgDisplay = GetBloodGroupDisplay(request.BloodGroup);

            foreach (var donor in matchingDonors)
            {
                if (donor.UserId == request.RequesterId) continue;
                if (donor.User?.FullName == acceptedDonorName) continue;

                await SendNotificationAsync(
                    donor.UserId,
                    $"✅ Blood Found - {bgDisplay}",
                    $"Good news! The {bgDisplay} blood request for {request.PatientName} at {request.HospitalName} has been fulfilled by another donor. Thank you for being ready to help!",
                    Domain.Enums.NotificationType.RequestCompleted,
                    actionUrl: $"/BloodRequest/Details/{bloodRequestId}");
            }
        }

        public async Task NotifyDonorsEmergencyFulfilledAsync(int emergencyRequestId, string acceptedDonorName)
        {
            var emergency = await _emergencyRequestRepository.GetByIdAsync(emergencyRequestId);
            if (emergency == null) return;

            var matchingDonors = await _donorRepository.GetDonorsByBloodGroupAsync(emergency.BloodGroup);
            var bgDisplay = GetBloodGroupDisplay(emergency.BloodGroup);

            foreach (var donor in matchingDonors)
            {
                if (donor.UserId == emergency.RequesterId) continue;
                if (donor.User?.FullName == acceptedDonorName) continue;

                await SendNotificationAsync(
                    donor.UserId,
                    $"✅ Blood Found - {bgDisplay}",
                    $"The emergency {bgDisplay} blood request for {emergency.PatientName} has been resolved by another donor. Thank you for your readiness to help!",
                    Domain.Enums.NotificationType.RequestCompleted,
                    actionUrl: $"/Emergency/Details/{emergencyRequestId}");
            }
        }

        private static string GetBloodGroupDisplay(Domain.Enums.BloodGroup bg) => bg switch
        {
            Domain.Enums.BloodGroup.APositive => "A+",
            Domain.Enums.BloodGroup.ANegative => "A-",
            Domain.Enums.BloodGroup.BPositive => "B+",
            Domain.Enums.BloodGroup.BNegative => "B-",
            Domain.Enums.BloodGroup.ABPositive => "AB+",
            Domain.Enums.BloodGroup.ABNegative => "AB-",
            Domain.Enums.BloodGroup.OPositive => "O+",
            Domain.Enums.BloodGroup.ONegative => "O-",
            _ => bg.ToString()
        };
    }
}
