using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.User;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IUserProfileService
    {
        Task<Result<UserProfileDto>> GetProfileAsync(string userId);
        Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<Result> UpdateProfileImageAsync(string userId, string imageUrl);
        Task<Result<List<LoginActivityDto>>> GetLoginActivityAsync(string userId);
        Task<Result<int>> GetProfileCompletionPercentageAsync(string userId);
    }
}