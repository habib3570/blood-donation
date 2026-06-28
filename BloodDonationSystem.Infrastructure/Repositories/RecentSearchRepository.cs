using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class RecentSearchRepository : GenericRepository<RecentSearch>, IRecentSearchRepository
    {
        public RecentSearchRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<RecentSearch>> GetByUserIdAsync(string userId, int count = 10)
            => await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SearchedAt)
                .Take(count)
                .ToListAsync();

        public async Task AddSearchAsync(string userId, string searchTerm, string? bloodGroup, string? district, string? upazila)
        {
            var existing = await _dbSet
                .FirstOrDefaultAsync(x => x.UserId == userId
                    && x.BloodGroup == bloodGroup
                    && x.District == district
                    && x.Upazila == upazila);

            if (existing != null)
            {
                existing.SearchedAt = DateTime.UtcNow;
                _dbSet.Update(existing);
            }
            else
            {
                await _dbSet.AddAsync(new RecentSearch
                {
                    UserId = userId,
                    SearchTerm = searchTerm,
                    BloodGroup = bloodGroup,
                    District = district,
                    Upazila = upazila,
                    SearchedAt = DateTime.UtcNow
                });
            }
        }

        public async Task ClearSearchHistoryAsync(string userId)
        {
            var searches = await _dbSet.Where(x => x.UserId == userId).ToListAsync();
            _dbSet.RemoveRange(searches);
        }
    }
}