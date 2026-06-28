using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class PointRepository : GenericRepository<UserPoints>, IPointRepository
    {
        public PointRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserPoints?> GetByUserIdAsync(string userId)
            => await _dbSet
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<List<PointTransaction>> GetTransactionsAsync(string userId, int page = 1, int pageSize = 20)
        {
            var userPoints = await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userPoints == null) return new List<PointTransaction>();

            return await _context.PointTransactions
                .Where(x => x.UserPointsId == userPoints.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddPointsAsync(string userId, int points, string reason, string? referenceId = null)
        {
            var userPoints = await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userPoints == null)
            {
                userPoints = new UserPoints { UserId = userId, TotalPoints = 0 };
                await _dbSet.AddAsync(userPoints);
                await _context.SaveChangesAsync();
            }

            userPoints.TotalPoints += points;
            userPoints.CurrentMonthPoints += points;
            userPoints.LastUpdated = DateTime.UtcNow;

            await _context.PointTransactions.AddAsync(new PointTransaction
            {
                UserPointsId = userPoints.Id,
                Points = points,
                Reason = reason,
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<List<UserPoints>> GetLeaderboardAsync(int count = 10)
            => await _dbSet
                .Include(x => x.User)
                    .ThenInclude(u => u.DonorProfile)
                .OrderByDescending(x => x.TotalPoints)
                .Take(count)
                .ToListAsync();

        public async Task<int> GetUserRankAsync(string userId)
        {
            var userPoints = await _dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
            if (userPoints == null) return 0;
            return await _dbSet.CountAsync(x => x.TotalPoints > userPoints.TotalPoints) + 1;
        }

        public async Task ResetMonthlyPointsAsync()
        {
            var allPoints = await _dbSet.ToListAsync();
            foreach (var p in allPoints) p.CurrentMonthPoints = 0;
        }
    }
}