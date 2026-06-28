using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class FavoriteDonorRepository : GenericRepository<FavoriteDonor>, IFavoriteDonorRepository
    {
        public FavoriteDonorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<FavoriteDonor>> GetFavoritesAsync(string userId)
            => await _dbSet
                .Include(x => x.DonorProfile)
                    .ThenInclude(d => d.User)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.AddedAt)
                .ToListAsync();

        public async Task<bool> IsFavoriteAsync(string userId, int donorProfileId)
            => await _dbSet
                .AnyAsync(x => x.UserId == userId && x.DonorProfileId == donorProfileId);

        public async Task RemoveFavoriteAsync(string userId, int donorProfileId)
        {
            var favorite = await _dbSet
                .FirstOrDefaultAsync(x => x.UserId == userId && x.DonorProfileId == donorProfileId);
            if (favorite != null)
                _dbSet.Remove(favorite);
        }
    }
}