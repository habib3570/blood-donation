using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
            => await _dbSet.FirstOrDefaultAsync(x => x.UserName == userName);

        public async Task<ApplicationUser?> GetWithProfileAsync(string userId)
            => await _dbSet
                .Include(x => x.DonorProfile)
                    .ThenInclude(d => d != null ? d.Badges : null)
                .Include(x => x.DonorProfile)
                    .ThenInclude(d => d != null ? d.Achievements : null)
                .Include(x => x.UserPoints)
                .Include(x => x.UserPreference)
                .FirstOrDefaultAsync(x => x.Id == userId);

        public async Task<List<ApplicationUser>> GetAllWithProfilesAsync()
            => await _dbSet
                .Include(x => x.DonorProfile)
                .Include(x => x.UserPoints)
                .Where(x => !x.IsBlocked)
                .ToListAsync();

        public async Task<List<ApplicationUser>> GetBlockedUsersAsync()
            => await _dbSet.Where(x => x.IsBlocked).ToListAsync();

        public async Task<int> GetTotalUsersCountAsync()
            => await _dbSet.CountAsync();

        public async Task<List<ApplicationUser>> GetUsersByDistrictAsync(string district)
            => await _dbSet.Where(x => x.District == district).ToListAsync();

        public async Task UpdateLastSeenAsync(string userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.LastSeenAt = DateTime.UtcNow;
                _dbSet.Update(user);
            }
        }
    }
}