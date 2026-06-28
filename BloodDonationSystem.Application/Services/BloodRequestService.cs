using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.BloodRequest;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BloodRequestService(
            IBloodRequestRepository bloodRequestRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _bloodRequestRepository = bloodRequestRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<BloodRequestDto>> CreateRequestAsync(string userId, CreateBloodRequestDto dto)
        {
            if (dto.IsEmergency)
            {
                var todayCount = await _bloodRequestRepository.GetTodayRequestCountByUserAsync(userId);
                if (todayCount >= DonationConstants.MaxEmergencyRequestsPerDay)
                    return Result<BloodRequestDto>.Failure($"You can only send {DonationConstants.MaxEmergencyRequestsPerDay} emergency requests per day.");
            }

            var request = _mapper.Map<BloodRequest>(dto);
            request.RequesterId = userId;
            request.Status = RequestStatus.Pending;
            request.CreatedAt = DateTime.UtcNow;

            if (dto.IsEmergency)
                request.ExpiryDate = DateTime.UtcNow.AddHours(DonationConstants.RequestExpiryHours);

            await _bloodRequestRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            var matchingDonors = await _donorRepository.SearchDonorsAsync(
                dto.BloodGroup, dto.District, dto.Upazila);

            foreach (var donor in matchingDonors.Take(20))
            {
                await _notificationService.SendNotificationAsync(
                    donor.UserId,
                    $"New Blood Request 🩸",
                    $"{dto.BloodGroup} blood needed at {dto.HospitalName}, {dto.District}",
                    NotificationType.NewBloodRequest,
                    $"/BloodRequest/Details/{request.Id}",
                    request.Id.ToString());
            }

            return Result<BloodRequestDto>.Success(_mapper.Map<BloodRequestDto>(request), "Blood request created successfully.");
        }

        public async Task<Result<BloodRequestDto>> GetRequestByIdAsync(int id)
        {
            var request = await _bloodRequestRepository.GetWithDetailsAsync(id);
            if (request == null)
                return Result<BloodRequestDto>.Failure("Request not found.");
            return Result<BloodRequestDto>.Success(_mapper.Map<BloodRequestDto>(request));
        }

        public async Task<Result<List<BloodRequestDto>>> GetAllActiveRequestsAsync()
        {
            var requests = await _bloodRequestRepository.GetActiveRequestsAsync();
            return Result<List<BloodRequestDto>>.Success(_mapper.Map<List<BloodRequestDto>>(requests));
        }

        public async Task<Result<List<BloodRequestDto>>> GetRequestsByUserAsync(string userId)
        {
            var requests = await _bloodRequestRepository.GetByRequesterIdAsync(userId);
            return Result<List<BloodRequestDto>>.Success(_mapper.Map<List<BloodRequestDto>>(requests));
        }

        public async Task<Result> AcceptRequestAsync(string donorId, int requestId)
        {
            var request = await _bloodRequestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");
            if (request.Status != RequestStatus.Pending)
                return Result.Failure("This request is no longer available.");

            var donorProfile = await _donorRepository.GetByUserIdAsync(donorId);
            if (donorProfile == null)
                return Result.Failure("Donor profile not found.");

            var isEligible = await _donorRepository.IsEligibleToDonateAsync(donorProfile.Id);
            if (!isEligible)
            {
                var days = await _donorRepository.GetDaysUntilEligibleAsync(donorProfile.Id);
                return Result.Failure($"You are not eligible to donate yet. {days} days remaining.");
            }

            request.DonorId = donorId;
            request.Status = RequestStatus.Accepted;
            request.AcceptedAt = DateTime.UtcNow;
            _bloodRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "Request Accepted! ✅",
                $"A donor has accepted your blood request for {request.PatientName}.",
                NotificationType.RequestAccepted,
                $"/BloodRequest/Details/{requestId}");

            return Result.Success("Request accepted. Please contact the requester.");
        }

        public async Task<Result> RejectRequestAsync(string donorId, int requestId)
        {
            var request = await _bloodRequestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");

            request.Status = RequestStatus.Rejected;
            _bloodRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "Request Update",
                "A donor was unable to accept your request. Looking for another donor...",
                NotificationType.RequestRejected);

            return Result.Success("Request rejected.");
        }

        public async Task<Result> CompleteRequestAsync(int requestId)
        {
            var request = await _bloodRequestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");

            request.Status = RequestStatus.Completed;
            request.CompletedAt = DateTime.UtcNow;
            _bloodRequestRepository.Update(request);

            if (request.DonorId != null)
            {
                var donorProfile = await _donorRepository.GetByUserIdAsync(request.DonorId);
                if (donorProfile != null)
                {
                    donorProfile.TotalDonations++;
                    donorProfile.LastDonationDate = DateTime.UtcNow;
                    donorProfile.NextEligibleDate = DateTime.UtcNow.AddDays(DonationConstants.MinDaysBetweenDonations);
                    donorProfile.LivesSaved += DonationConstants.LivessavedPerDonation;
                    _donorRepository.Update(donorProfile);

                    await _gamificationService.AddPointsAsync(request.DonorId, PointConstants.DonationPoints, "Blood Donation Completed", requestId.ToString());
                    await _gamificationService.CheckAndAwardBadgesAsync(donorProfile.Id);
                    await _gamificationService.CheckAndAwardAchievementsAsync(donorProfile.Id);
                    await _gamificationService.UpdateDonorLevelAsync(donorProfile.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "Donation Completed! 🎉",
                "The blood donation has been completed. Please rate your donor.",
                NotificationType.RequestCompleted,
                $"/Rating/Rate/{request.DonorId}");

            return Result.Success("Request marked as completed.");
        }

        public async Task<Result> CancelRequestAsync(string userId, int requestId)
        {
            var request = await _bloodRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");
            if (request.RequesterId != userId)
                return Result.Failure("Unauthorized.");

            request.Status = RequestStatus.Cancelled;
            _bloodRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Request cancelled.");
        }

        public async Task<Result> ReportRequestAsync(string userId, int requestId, string reason)
        {
            var request = await _bloodRequestRepository.GetByIdAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");

            request.IsReported = true;
            _bloodRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Request reported successfully.");
        }

        public async Task<Result> ExpireOldRequestsAsync()
        {
            var expired = await _bloodRequestRepository.GetExpiredRequestsAsync();
            foreach (var r in expired)
            {
                r.Status = RequestStatus.Expired;
                _bloodRequestRepository.Update(r);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success($"{expired.Count} requests expired.");
        }

        public async Task<Result<int>> GetTodayRequestCountAsync(string userId)
        {
            var count = await _bloodRequestRepository.GetTodayRequestCountByUserAsync(userId);
            return Result<int>.Success(count);
        }
    }
}