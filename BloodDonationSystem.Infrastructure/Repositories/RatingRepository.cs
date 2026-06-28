using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class RatingRepository : GenericRepository<DonorRating>, IRatingRepository
    {
        public RatingRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<DonorRating>> GetByDonorProfileIdAsync(int donorProfileId)
            => await _dbSet
                .Include(x => x.Rater)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<double> GetAverageRatingAsync(int donorProfileId)
        {
            var ratings = await _dbSet
                .Where(x => x.DonorProfileId == donorProfileId)
                .ToListAsync();
            return ratings.Any() ? ratings.Average(x => x.Stars) : 0;
        }

        public async Task<bool> HasRatedAsync(string raterId, int donorProfileId)
            => await _dbSet.AnyAsync(x => x.RaterId == raterId && x.DonorProfileId == donorProfileId);

        public async Task<List<DonorRating>> GetRecentRatingsAsync(int donorProfileId, int count = 5)
            => await _dbSet
                .Include(x => x.Rater)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToListAsync();
    }
}