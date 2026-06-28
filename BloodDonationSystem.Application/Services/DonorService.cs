using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DonorService(
            IDonorRepository donorRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _donorRepository = donorRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _gamificationService = gamificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DonorDto>> GetDonorByUserIdAsync(string userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<DonorDto>.Failure("Donor profile not found.");
            return Result<DonorDto>.Success(_mapper.Map<DonorDto>(donor));
        }

        public async Task<Result<DonorDto>> GetDonorByIdAsync(int id)
        {
            var donor = await _donorRepository.GetWithDetailsAsync(id);
            if (donor == null)
                return Result<DonorDto>.Failure("Donor not found.");
            return Result<DonorDto>.Success(_mapper.Map<DonorDto>(donor));
        }

        public async Task<Result> BecomeDonorAsync(string userId)
        {
            var existing = await _donorRepository.GetByUserIdAsync(userId);
            if (existing != null)
                return Result.Failure("You are already registered as a donor.");

            var donor = new DonorProfile
            {
                UserId = userId,
                IsAvailable = true,
                Level = DonorLevel.NewDonor,
                CreatedAt = DateTime.UtcNow
            };

            await _donorRepository.AddAsync(donor);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                userId,
                "Welcome Donor! 🩸",
                "You are now registered as a blood donor. Thank you for saving lives!",
                NotificationType.SystemAlert);

            return Result.Success("You are now a registered donor!");
        }

        public async Task<Result> UpdateDonorProfileAsync(string userId, UpdateDonorProfileDto dto)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result.Failure("Donor profile not found.");

            donor.PreferredArea = dto.PreferredArea;
            donor.PreferredDistrict = dto.PreferredDistrict;
            donor.PreferredUpazila = dto.PreferredUpazila;
            donor.IsEmergencyOnly = dto.IsEmergencyOnly;
            donor.IsVacationMode = dto.IsVacationMode;
            donor.VacationEndDate = dto.VacationEndDate;
            donor.UpdatedAt = DateTime.UtcNow;

            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Donor profile updated.");
        }

        public async Task<Result> ToggleAvailabilityAsync(string userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result.Failure("Donor profile not found.");

            donor.IsAvailable = !donor.IsAvailable;
            donor.UpdatedAt = DateTime.UtcNow;

            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();

            var status = donor.IsAvailable ? "Available" : "Unavailable";
            return Result.Success($"You are now marked as {status}.");
        }

        public async Task<Result> SetVacationModeAsync(string userId, DateTime? endDate)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result.Failure("Donor profile not found.");

            donor.IsVacationMode = endDate.HasValue;
            donor.VacationEndDate = endDate;
            donor.IsAvailable = !endDate.HasValue;
            donor.UpdatedAt = DateTime.UtcNow;

            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(endDate.HasValue ? $"Vacation mode enabled until {endDate:dd MMM yyyy}." : "Vacation mode disabled.");
        }

        public async Task<Result> SetEmergencyOnlyModeAsync(string userId, bool emergencyOnly)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result.Failure("Donor profile not found.");

            donor.IsEmergencyOnly = emergencyOnly;
            donor.UpdatedAt = DateTime.UtcNow;

            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(emergencyOnly ? "Emergency-only mode enabled." : "Emergency-only mode disabled.");
        }

        public async Task<Result<List<DonorDto>>> SearchDonorsAsync(DonorFilterDto filter)
        {
            List<DonorProfile> donors;

            if (filter.Latitude.HasValue && filter.Longitude.HasValue && filter.RadiusKm.HasValue)
            {
                donors = await _donorRepository.GetNearbyDonorsAsync(
                    filter.Latitude.Value,
                    filter.Longitude.Value,
                    filter.RadiusKm.Value,
                    filter.BloodGroup);
            }
            else if (filter.BloodGroup.HasValue && !string.IsNullOrEmpty(filter.District))
            {
                donors = await _donorRepository.SearchDonorsAsync(
                    filter.BloodGroup.Value,
                    filter.District,
                    filter.Upazila ?? "",
                    filter.Gender);
            }
            else
            {
                donors = (await _donorRepository.GetAvailableDonorsAsync()).ToList();
            }

            if (filter.IsVerified == true)
                donors = donors.Where(d => d.IsVerifiedDonor).ToList();

            var dtos = _mapper.Map<List<DonorDto>>(donors);
            return Result<List<DonorDto>>.Success(dtos);
        }

        public async Task<Result<List<DonorDto>>> GetNearbyDonorsAsync(double latitude, double longitude, double radiusKm, BloodGroup? bloodGroup = null)
        {
            var donors = await _donorRepository.GetNearbyDonorsAsync(latitude, longitude, radiusKm, bloodGroup);
            return Result<List<DonorDto>>.Success(_mapper.Map<List<DonorDto>>(donors));
        }

        public async Task<Result<List<DonorDto>>> GetTopDonorsAsync(int count = 10)
        {
            var donors = await _donorRepository.GetTopDonorsByPointsAsync(count);
            return Result<List<DonorDto>>.Success(_mapper.Map<List<DonorDto>>(donors));
        }

        public async Task<Result<DonorAvailabilityDto>> CheckAvailabilityAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor == null)
                return Result<DonorAvailabilityDto>.Failure("Donor not found.");

            var isEligible = await _donorRepository.IsEligibleToDonateAsync(donorProfileId);
            var daysUntil = await _donorRepository.GetDaysUntilEligibleAsync(donorProfileId);

            return Result<DonorAvailabilityDto>.Success(new DonorAvailabilityDto
            {
                IsAvailable = donor.IsAvailable,
                IsEligible = isEligible,
                IsVacationMode = donor.IsVacationMode,
                IsEmergencyOnly = donor.IsEmergencyOnly,
                NextEligibleDate = donor.NextEligibleDate,
                DaysUntilEligible = daysUntil > 0 ? daysUntil : null,
                IneligibilityReason = !isEligible ? $"Last donation was less than {DonationConstants.MinDaysBetweenDonations} days ago." : null
            });
        }

        public async Task<Result> UpdateSmartPriorityScoresAsync()
        {
            var donors = await _donorRepository.GetAvailableDonorsAsync();
            foreach (var donor in donors)
            {
                double score = 0;
                if (donor.IsVerifiedDonor) score += 30;
                if (donor.IsAvailable && !donor.IsVacationMode) score += 25;
                score += Math.Min(donor.AverageRating * 8, 40);
                if (donor.LastDonationDate.HasValue)
                {
                    var daysSince = (DateTime.UtcNow - donor.LastDonationDate.Value).TotalDays;
                    if (daysSince >= DonationConstants.MinDaysBetweenDonations)
                        score += 20;
                }
                score += Math.Min(donor.TotalDonations * 0.5, 15);
                await _donorRepository.UpdateSmartPriorityScoreAsync(donor.Id, score);
            }
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Smart priority scores updated.");
        }

        public async Task<Result> VerifyDonorAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor == null)
                return Result.Failure("Donor not found.");

            donor.IsVerifiedDonor = true;
            donor.UpdatedAt = DateTime.UtcNow;
            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                donor.UserId,
                "Donor Verified ✅",
                "Congratulations! Your donor profile has been verified by admin.",
                NotificationType.SystemAlert);

            return Result.Success("Donor verified successfully.");
        }
    }
}