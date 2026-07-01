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
        private readonly IUserRepository _userRepository;
        private readonly IDonationRepository _donationRepository; 

        public BloodRequestService(
            IBloodRequestRepository bloodRequestRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserRepository userRepository,
            IDonationRepository donationRepository) 
        {
            _bloodRequestRepository = bloodRequestRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
            _donationRepository = donationRepository; 
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

            await _notificationService.NotifyMatchingDonorsForRequestAsync(request.Id);

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

            var donor = await _userRepository.FirstOrDefaultAsync(u => u.Id == donorId);
            await _notificationService.NotifyDonorsRequestFulfilledAsync(requestId, donor?.FullName ?? "A donor");

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "✅ Donor Found!",
                $"{donor?.FullName ?? "A donor"} has accepted your blood request for {request.PatientName}.",
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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var request = await _bloodRequestRepository.GetWithDetailsAsync(requestId);
                if (request == null)
                    return Result.Failure("Request not found.");

                if (request.Status != RequestStatus.Accepted)
                    return Result.Failure("This request cannot be marked as completed.");

                request.Status = RequestStatus.Completed;
                request.CompletedAt = DateTime.UtcNow;
                _bloodRequestRepository.Update(request);

                int donorProfileId = 0;
                string? donorUserId = request.DonorId;

                if (donorUserId != null)
                {
                    var donorProfile = await _donorRepository.GetByUserIdAsync(donorUserId);
                    if (donorProfile != null)
                    {
                        donorProfileId = donorProfile.Id;

                        // Donation row তৈরি করো
                        var donation = new Donation
                        {
                            DonorProfileId = donorProfile.Id,
                            BloodRequestId = request.Id,
                            RecipientName = request.PatientName,
                            HospitalName = request.HospitalName,
                            District = request.District,
                            DonationDate = DateTime.UtcNow,
                            IsEmergency = request.IsEmergency,
                            CertificateGenerated = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _donationRepository.AddAsync(donation);

                        // DonorProfile আপডেট
                        donorProfile.TotalDonations++;
                        donorProfile.LastDonationDate = DateTime.UtcNow;
                        donorProfile.NextEligibleDate = DateTime.UtcNow.AddDays(
                            DonationConstants.MinDaysBetweenDonations);
                        donorProfile.LivesSaved += DonationConstants.LivessavedPerDonation;
                        donorProfile.IsAvailable = false; 
                        donorProfile.UpdatedAt = DateTime.UtcNow;
                        _donorRepository.Update(donorProfile);
                    }
                }

               
                await _unitOfWork.SaveChangesAsync();

                // Transaction commit করো
                await _unitOfWork.CommitTransactionAsync();


                // SaveChanges এর পরে Gamification চালাও
                if (donorUserId != null && donorProfileId > 0)
                {
                    try
                    {
                        await _gamificationService.AddPointsAsync(
                            donorUserId,
                            PointConstants.DonationPoints,
                            "Blood Donation Completed",
                            requestId.ToString());

                        await _gamificationService.CheckAndAwardBadgesAsync(donorProfileId);
                        await _gamificationService.CheckAndAwardAchievementsAsync(donorProfileId);
                        await _gamificationService.UpdateDonorLevelAsync(donorProfileId);

                        // Gamification এর পরে IsAvailable নিশ্চিত করো
                        var donorCheck = await _donorRepository.GetByUserIdAsync(donorUserId);
                        if (donorCheck != null && donorCheck.IsAvailable)
                        {
                            donorCheck.IsAvailable = false;
                            donorCheck.UpdatedAt = DateTime.UtcNow;
                            _donorRepository.Update(donorCheck);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        try
                        {
                            var donorFallback = await _donorRepository.GetByUserIdAsync(donorUserId);
                            if (donorFallback != null)
                            {
                                donorFallback.IsAvailable = false;
                                donorFallback.UpdatedAt = DateTime.UtcNow;
                                _donorRepository.Update(donorFallback);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }
                        catch { }
                    }
                }

                return Result.Success("Request marked as completed.");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure("Failed to complete the request. Please try again.");
            }
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

        public async Task<Result<List<BloodRequestDto>>> GetAcceptedRequestsByDonorAsync(string donorId)
        {
            var requests = await _bloodRequestRepository.GetAcceptedRequestsByDonorAsync(donorId);
            return Result<List<BloodRequestDto>>.Success(_mapper.Map<List<BloodRequestDto>>(requests));
        }

        public async Task<Result> CancelAcceptedRequestByDonorAsync(string donorId, int requestId)
        {
            var request = await _bloodRequestRepository.GetWithDetailsAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");

            if (request.DonorId != donorId)
                return Result.Failure("Unauthorized. You did not accept this request.");

            if (request.Status != RequestStatus.Accepted)
                return Result.Failure("This request cannot be cancelled.");

            var donor = await _userRepository.FirstOrDefaultAsync(u => u.Id == donorId);

            request.Status = RequestStatus.Pending;
            request.DonorId = null;
            request.AcceptedAt = null;
            _bloodRequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "⚠️ Donor Cancelled",
                $"{donor?.FullName ?? "The donor"} has cancelled their acceptance for {request.PatientName}'s blood request. We are looking for another donor.",
                NotificationType.RequestRejected,
                $"/BloodRequest/Details/{requestId}");

            return Result.Success("You have cancelled this acceptance. The requester has been notified.");
        }
    }
}