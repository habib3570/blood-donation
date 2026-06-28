using BloodDonationSystem.Application.Common.Models;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IGamificationService
    {
        Task<Result> AddPointsAsync(string userId, int points, string reason, string? referenceId = null);
        Task<Result> CheckAndAwardBadgesAsync(int donorProfileId);
        Task<Result> CheckAndAwardAchievementsAsync(int donorProfileId);
        Task<Result> UpdateDonorLevelAsync(int donorProfileId);
        Task<Result> UpdateStreakAsync(int donorProfileId);
        Task<Result> UpdateMonthlyTopDonorsAsync();
        Task<Result<int>> GetUserRankAsync(string userId);
        Task<Result<int>> GetUserPointsAsync(string userId);
    }
}