using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Achievement;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IAchievementService
    {
        Task<Result<List<AchievementDto>>> GetAllAchievementsAsync();
        Task<Result<List<UserAchievementDto>>> GetUserAchievementsAsync(
            int donorProfileId);
    }
}