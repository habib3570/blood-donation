using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class AchievementRepository : GenericRepository<Achievement>, IAchievementRepository
    {
        public AchievementRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<UserAchievement>> GetUserAchievementsAsync(int donorProfileId)
            => await _context.UserAchievements
                .Include(x => x.Achievement)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.EarnedAt)
                .ToListAsync();

        public async Task<bool> HasAchievementAsync(int donorProfileId, int achievementId)
            => await _context.UserAchievements
                .AnyAsync(x => x.DonorProfileId == donorProfileId && x.AchievementId == achievementId);

        public async Task<List<Achievement>> GetAllAchievementsAsync()
            => await _dbSet.OrderBy(x => x.RequiredCount).ToListAsync();

        public async Task AddUserAchievementAsync(UserAchievement userAchievement)
            => await _context.UserAchievements.AddAsync(userAchievement);
    }
}