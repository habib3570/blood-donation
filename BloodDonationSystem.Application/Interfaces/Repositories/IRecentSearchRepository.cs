using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IRecentSearchRepository : IGenericRepository<RecentSearch>
    {
        Task<List<RecentSearch>> GetByUserIdAsync(string userId, int count = 10);
        Task AddSearchAsync(string userId, string searchTerm, string? bloodGroup, string? district, string? upazila);
        Task ClearSearchHistoryAsync(string userId);
    }
}