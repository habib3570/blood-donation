using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.User;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginActivityRepository _loginActivityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserProfileService(
            IUserRepository userRepository,
            ILoginActivityRepository loginActivityRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _loginActivityRepository = loginActivityRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UserProfileDto>> GetProfileAsync(string userId)
        {
            var user = await _userRepository.GetWithProfileAsync(userId);
            if (user == null)
                return Result<UserProfileDto>.Failure("User not found.");

            var completion = CalculateCompletionPercentage(user);

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
                ProfileCompletionPercentage = completion,
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

        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Result.Failure("User not found.");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.BloodGroup = dto.BloodGroup;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.District = dto.District;
            user.Upazila = dto.Upazila;
            user.Address = dto.Address;

            if (dto.Latitude.HasValue) user.Latitude = dto.Latitude;
            if (dto.Longitude.HasValue) user.Longitude = dto.Longitude;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Profile updated successfully.");
        }

        public async Task<Result> UpdateProfileImageAsync(string userId, string imageUrl)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Result.Failure("User not found.");

            user.ProfileImageUrl = imageUrl;
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Profile image updated.");
        }

        public async Task<Result<List<LoginActivityDto>>> GetLoginActivityAsync(
            string userId)
        {
            var activities = await _loginActivityRepository.GetByUserIdAsync(userId);
            var dtos = activities.Select(a => new LoginActivityDto
            {
                Id = a.Id,
                IpAddress = a.IpAddress,
                DeviceInfo = a.DeviceInfo,
                Browser = a.Browser,
                IsSuccessful = a.IsSuccessful,
                LoginAt = a.LoginAt
            }).ToList();

            return Result<List<LoginActivityDto>>.Success(dtos);
        }

        public async Task<Result<int>> GetProfileCompletionPercentageAsync(string userId)
        {
            var user = await _userRepository.GetWithProfileAsync(userId);
            if (user == null)
                return Result<int>.Success(0);

            return Result<int>.Success(CalculateCompletionPercentage(user));
        }

        private static int CalculateCompletionPercentage(
            Domain.Entities.ApplicationUser user)
        {
            int totalFields = 8;
            int filledFields = 0;

            if (!string.IsNullOrEmpty(user.FullName)) filledFields++;
            if (!string.IsNullOrEmpty(user.PhoneNumber)) filledFields++;
            if (!string.IsNullOrEmpty(user.ProfileImageUrl)) filledFields++;
            if (!string.IsNullOrEmpty(user.District)) filledFields++;
            if (!string.IsNullOrEmpty(user.Upazila)) filledFields++;
            if (!string.IsNullOrEmpty(user.Address)) filledFields++;
            if (user.Latitude.HasValue && user.Longitude.HasValue) filledFields++;
            if (user.DonorProfile != null) filledFields++;

            return (int)((filledFields / (double)totalFields) * 100);
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