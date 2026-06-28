using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Emergency;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class EmergencyService : IEmergencyService
    {
        private readonly IEmergencyRequestRepository _emergencyRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmergencyService(
            IEmergencyRequestRepository emergencyRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _emergencyRepository = emergencyRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<EmergencyRequestDto>> CreateEmergencyRequestAsync(string userId, CreateEmergencyRequestDto dto)
        {
            var todayCount = await _emergencyRepository.GetTodayEmergencyCountByUserAsync(userId);
            if (todayCount >= DonationConstants.MaxEmergencyRequestsPerDay)
                return Result<EmergencyRequestDto>.Failure($"Maximum {DonationConstants.MaxEmergencyRequestsPerDay} emergency requests allowed per day.");

            var request = _mapper.Map<EmergencyRequest>(dto);
            request.RequesterId = userId;
            request.Status = RequestStatus.Pending;
            request.IsActive = true;
            request.ExpiryDate = DateTime.UtcNow.AddHours(DonationConstants.RequestExpiryHours);
            request.CreatedAt = DateTime.UtcNow;

            await _emergencyRepository.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            await NotifyNearbyDonorsAsync(request.Id);

            return Result<EmergencyRequestDto>.Success(_mapper.Map<EmergencyRequestDto>(request), "Emergency request sent!");
        }

        public async Task<Result<List<EmergencyRequestDto>>> GetActiveEmergencyRequestsAsync()
        {
            var requests = await _emergencyRepository.GetActiveEmergencyRequestsAsync();
            return Result<List<EmergencyRequestDto>>.Success(_mapper.Map<List<EmergencyRequestDto>>(requests));
        }

        public async Task<Result<EmergencyRequestDto>> GetEmergencyRequestByIdAsync(int id)
        {
            var request = await _emergencyRepository.GetWithAcceptancesAsync(id);
            if (request == null)
                return Result<EmergencyRequestDto>.Failure("Emergency request not found.");
            return Result<EmergencyRequestDto>.Success(_mapper.Map<EmergencyRequestDto>(request));
        }

        public async Task<Result> AcceptEmergencyRequestAsync(string donorId, int requestId)
        {
            var request = await _emergencyRepository.GetByIdAsync(requestId);
            if (request == null || !request.IsActive)
                return Result.Failure("Emergency request not found or expired.");

            var donorProfile = await _donorRepository.GetByUserIdAsync(donorId);
            if (donorProfile == null)
                return Result.Failure("Donor profile not found.");

            var isEligible = await _donorRepository.IsEligibleToDonateAsync(donorProfile.Id);
            if (!isEligible)
                return Result.Failure("You are not eligible to donate at this time.");

            request.Status = RequestStatus.Accepted;
            _emergencyRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                request.RequesterId,
                "Emergency Help Coming! 🚨",
                "A donor is on the way to help you!",
                NotificationType.RequestAccepted,
                $"/Emergency/Details/{requestId}");

            await _gamificationService.AddPointsAsync(donorId, PointConstants.EmergencyDonationPoints, "Emergency Request Accepted", requestId.ToString());

            return Result.Success("Emergency request accepted. Please proceed to the hospital immediately.");
        }

        public async Task<Result> CompleteEmergencyRequestAsync(int requestId)
        {
            var request = await _emergencyRepository.GetByIdAsync(requestId);
            if (request == null)
                return Result.Failure("Request not found.");

            request.Status = RequestStatus.Completed;
            request.IsActive = false;
            _emergencyRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Emergency request completed.");
        }

        public async Task<Result<bool>> CanSendEmergencyRequestAsync(string userId)
        {
            var count = await _emergencyRepository.GetTodayEmergencyCountByUserAsync(userId);
            return Result<bool>.Success(count < DonationConstants.MaxEmergencyRequestsPerDay);
        }

        public async Task<Result> NotifyNearbyDonorsAsync(int emergencyRequestId)
        {
            var request = await _emergencyRepository.GetByIdAsync(emergencyRequestId);
            if (request == null) return Result.Failure("Request not found.");

            List<DonorProfile> donors;

            if (request.HospitalLatitude.HasValue && request.HospitalLongitude.HasValue)
            {
                donors = await _donorRepository.GetNearbyDonorsAsync(
                    request.HospitalLatitude.Value,
                    request.HospitalLongitude.Value,
                    DonationConstants.NearbyRadiusKm10,
                    request.BloodGroup);
            }
            else
            {
                donors = await _donorRepository.SearchDonorsAsync(
                    request.BloodGroup, request.District, request.Upazila);
            }

            var bloodGroupDisplay = GetBloodGroupDisplay(request.BloodGroup);

            foreach (var donor in donors)
            {
                if (donor.IsEmergencyOnly || !donor.IsEmergencyOnly)
                {
                    await _notificationService.SendNotificationAsync(
                        donor.UserId,
                        $"🚨 EMERGENCY: {bloodGroupDisplay} Blood Needed!",
                        $"Patient: {request.PatientName} at {request.HospitalName}, {request.District}. Contact: {request.ContactNumber}",
                        NotificationType.EmergencyAlert,
                        $"/Emergency/Details/{emergencyRequestId}",
                        emergencyRequestId.ToString());
                }
            }

            request.NotificationsSent = donors.Count;
            _emergencyRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success($"Notified {donors.Count} donors.");
        }

        public async Task<Result> ExpireOldEmergencyRequestsAsync()
        {
            var expired = await _emergencyRepository.GetExpiredRequestsAsync();
            foreach (var r in expired)
            {
                r.IsActive = false;
                r.Status = RequestStatus.Expired;
                _emergencyRepository.Update(r);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success($"{expired.Count} emergency requests expired.");
        }

        private static string GetBloodGroupDisplay(BloodGroup bg) => bg switch
        {
            BloodGroup.APositive => "A+",
            BloodGroup.ANegative => "A-",
            BloodGroup.BPositive => "B+",
            BloodGroup.BNegative => "B-",
            BloodGroup.ABPositive => "AB+",
            BloodGroup.ABNegative => "AB-",
            BloodGroup.OPositive => "O+",
            BloodGroup.ONegative => "O-",
            _ => bg.ToString()
        };
    }
}