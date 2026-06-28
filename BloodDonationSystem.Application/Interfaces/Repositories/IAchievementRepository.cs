using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IAchievementRepository : IGenericRepository<Achievement>
    {
        Task<List<UserAchievement>> GetUserAchievementsAsync(int donorProfileId);
        Task<bool> HasAchievementAsync(int donorProfileId, int achievementId);
        Task<List<Achievement>> GetAllAchievementsAsync();
        Task AddUserAchievementAsync(UserAchievement userAchievement);
    }
}