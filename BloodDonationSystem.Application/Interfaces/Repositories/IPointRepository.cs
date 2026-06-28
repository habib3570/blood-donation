using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IPointRepository : IGenericRepository<UserPoints>
    {
        Task<UserPoints?> GetByUserIdAsync(string userId);
        Task<List<PointTransaction>> GetTransactionsAsync(string userId, int page = 1, int pageSize = 20);
        Task AddPointsAsync(string userId, int points, string reason, string? referenceId = null);
        Task<List<UserPoints>> GetLeaderboardAsync(int count = 10);
        Task<int> GetUserRankAsync(string userId);
        Task ResetMonthlyPointsAsync();
    }
}