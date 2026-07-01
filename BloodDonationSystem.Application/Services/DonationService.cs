using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donation;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonationService(
            IDonationRepository donationRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _donationRepository = donationRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> RecordDonationAsync(
            int donorProfileId,
            int? bloodRequestId,
            string hospitalName,
            string district,
            bool isEmergency = false)
        {
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor == null)
                return Result.Failure("Donor profile not found.");

            var isEligible = await _donorRepository.IsEligibleToDonateAsync(donorProfileId);
            if (!isEligible)
            {
                var days = await _donorRepository.GetDaysUntilEligibleAsync(donorProfileId);
                return Result.Failure($"Not eligible. {days} days remaining.");
            }

            var donation = new Donation
            {
                DonorProfileId = donorProfileId,
                BloodRequestId = bloodRequestId,
                RecipientName = "Patient",
                HospitalName = hospitalName,
                District = district,
                DonationDate = DateTime.UtcNow,
                IsEmergency = isEmergency,
                CreatedAt = DateTime.UtcNow
            };

            await _donationRepository.AddAsync(donation);

            donor.TotalDonations++;
            donor.LastDonationDate = DateTime.UtcNow;
            donor.NextEligibleDate = DateTime.UtcNow.AddDays(
                DonationConstants.MinDaysBetweenDonations);
            donor.LivesSaved += DonationConstants.LivessavedPerDonation;
            donor.UpdatedAt = DateTime.UtcNow;
            _donorRepository.Update(donor);

            await _unitOfWork.SaveChangesAsync();

            var points = isEmergency
                ? PointConstants.EmergencyDonationPoints
                : PointConstants.DonationPoints;

            await _gamificationService.AddPointsAsync(
                donor.UserId, points,
                isEmergency ? "Emergency Donation" : "Blood Donation",
                donation.Id.ToString());

            await _gamificationService.CheckAndAwardBadgesAsync(donorProfileId);
            await _gamificationService.CheckAndAwardAchievementsAsync(donorProfileId);
            await _gamificationService.UpdateDonorLevelAsync(donorProfileId);
            await _gamificationService.UpdateStreakAsync(donorProfileId);

            await _notificationService.SendNotificationAsync(
                donor.UserId,
                "Donation Recorded! 🩸",
                $"Thank you! Your donation at {hospitalName} has been recorded. " +
                $"+{points} points earned!",
                Domain.Enums.NotificationType.RequestCompleted);

            return Result.Success("Donation recorded successfully.");
        }

        public async Task<Result<List<DonationHistoryDto>>> GetDonationHistoryAsync(
            int donorProfileId)
        {
            var donations = await _donationRepository.GetDonationHistoryAsync(donorProfileId);
            var dtos = _mapper.Map<List<DonationHistoryDto>>(donations);
            return Result<List<DonationHistoryDto>>.Success(dtos);
        }

        public async Task<Result<DonationDto>> GetLastDonationAsync(int donorProfileId)
        {
            var donation = await _donationRepository.GetLastDonationAsync(donorProfileId);
            if (donation == null)
                return Result<DonationDto>.Failure("No donations found.");
            return Result<DonationDto>.Success(_mapper.Map<DonationDto>(donation));
        }

        public async Task<Result<int>> GetTotalDonationsAsync()
        {
            var count = await _donationRepository.GetTotalDonationsCountAsync();
            return Result<int>.Success(count);
        }

        public async Task<Result<Dictionary<int, int>>> GetMonthlyStatsAsync(
            int donorProfileId, int year)
        {
            var stats = await _donationRepository.GetMonthlyDonationStatsAsync(
                donorProfileId, year);
            return Result<Dictionary<int, int>>.Success(stats);
        }

        public async Task<Result<int>> GetDaysUntilNextDonationAsync(int donorProfileId)
        {
            var days = await _donorRepository.GetDaysUntilEligibleAsync(donorProfileId);
            return Result<int>.Success(days);
        }

        public async Task<Result> UpdateDonorLevelAsync(int donorProfileId)
        {
            await _gamificationService.UpdateDonorLevelAsync(donorProfileId);
            return Result.Success();
        }

        public async Task<Result> UpdateDonationStreakAsync(int donorProfileId)
        {
            await _gamificationService.UpdateStreakAsync(donorProfileId);
            return Result.Success();
        }
        public async Task<Result<List<DonationHistoryDto>>> GetDonationHistoryByUserIdAsync(string userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<List<DonationHistoryDto>>.Failure("Donor profile not found.");

            var donations = await _donationRepository.GetDonationHistoryAsync(donor.Id);
            var dtos = _mapper.Map<List<DonationHistoryDto>>(donations);
            return Result<List<DonationHistoryDto>>.Success(dtos);
        }
    }
}