using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.User;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BloodDonationSystem.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(
            IUserRepository userRepository,
            IDonorRepository donorRepository,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _donorRepository = donorRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<UserProfileDto>>> GetAllUsersAsync(
            int page = 1, int pageSize = 20)
        {
            var users = await _userRepository.GetAllWithProfilesAsync();
            var paged = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = paged.Select(u => new UserProfileDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                PhoneNumber = u.PhoneNumber,
                BloodGroup = u.BloodGroup,
                BloodGroupDisplay = GetBloodGroupDisplay(u.BloodGroup),
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                Age = DateTime.Now.Year - u.DateOfBirth.Year,
                District = u.District,
                Upazila = u.Upazila,
                IsVerified = u.IsVerified,
                IsBlocked = u.IsBlocked,
                LastSeenAt = u.LastSeenAt,
                CreatedAt = u.CreatedAt,
                ProfileImageUrl = u.ProfileImageUrl,
                DonorInfo = u.DonorProfile != null ? new DonorInfoDto
                {
                    DonorProfileId = u.DonorProfile.Id,
                    IsAvailable = u.DonorProfile.IsAvailable,
                    TotalDonations = u.DonorProfile.TotalDonations,
                    AverageRating = u.DonorProfile.AverageRating,
                    Level = u.DonorProfile.Level.ToString(),
                    TotalPoints = u.DonorProfile.TotalPoints,
                    LivesSaved = u.DonorProfile.LivesSaved,
                    NextEligibleDate = u.DonorProfile.NextEligibleDate
                } : null
            }).ToList();

            return Result<List<UserProfileDto>>.Success(dtos);
        }

        public async Task<Result> BlockUserAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                return Result.Failure("User not found.");

            appUser.IsBlocked = true;
            await _userManager.UpdateAsync(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                userId,
                "Account Blocked",
                "Your account has been blocked. Contact support for assistance.",
                Domain.Enums.NotificationType.SystemAlert);

            return Result.Success("User blocked successfully.");
        }

        public async Task<Result> UnblockUserAsync(string userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                return Result.Failure("User not found.");

            appUser.IsBlocked = false;
            await _userManager.UpdateAsync(appUser);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendNotificationAsync(
                userId,
                "Account Unblocked ✅",
                "Your account has been unblocked. Welcome back!",
                Domain.Enums.NotificationType.SystemAlert);

            return Result.Success("User unblocked successfully.");
        }

       

        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found.");

            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("User deleted.");
        }

        public async Task<Result<UserProfileDto>> GetUserDetailsAsync(string userId)
        {
            var user = await _userRepository.GetWithProfileAsync(userId);
            if (user == null)
                return Result<UserProfileDto>.Failure("User not found.");

            var dto = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                BloodGroup = user.BloodGroup,
                BloodGroupDisplay = GetBloodGroupDisplay(user.BloodGroup),
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Age = DateTime.Now.Year - user.DateOfBirth.Year,
                District = user.District,
                Upazila = user.Upazila,
                IsVerified = user.IsVerified,
                IsBlocked = user.IsBlocked,
                LastSeenAt = user.LastSeenAt,
                CreatedAt = user.CreatedAt,
                ProfileImageUrl = user.ProfileImageUrl,
                DonorInfo = user.DonorProfile != null ? new DonorInfoDto
                {
                    DonorProfileId = user.DonorProfile.Id,
                    IsAvailable = user.DonorProfile.IsAvailable,
                    TotalDonations = user.DonorProfile.TotalDonations,
                    AverageRating = user.DonorProfile.AverageRating,
                    Level = user.DonorProfile.Level.ToString(),
                    TotalPoints = user.DonorProfile.TotalPoints,
                    LivesSaved = user.DonorProfile.LivesSaved,
                    NextEligibleDate = user.DonorProfile.NextEligibleDate
                } : null
            };

            return Result<UserProfileDto>.Success(dto);
        }

        public async Task<Result<List<object>>> GetSystemReportsAsync()
        {
            return Result<List<object>>.Success(new List<object>());
        }

        public async Task<Result> ReviewRequestReportAsync(int reportId, bool isFake)
        {
            return Result.Success("Report reviewed.");
        }

        public async Task<Result<List<object>>> GetMostActiveDistrictsAsync()
        {
            return Result<List<object>>.Success(new List<object>());
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
                "Donor Verified! ✅",
                "Congratulations! Your donor profile has been verified by admin.",
                Domain.Enums.NotificationType.SystemAlert);

            return Result.Success("Donor verified successfully.");
        }
    }
}