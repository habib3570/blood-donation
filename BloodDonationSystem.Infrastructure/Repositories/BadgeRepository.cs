using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class BadgeRepository : GenericRepository<Badge>, IBadgeRepository
    {
        public BadgeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<UserBadge>> GetUserBadgesAsync(int donorProfileId)
            => await _context.UserBadges
                .Include(x => x.Badge)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.EarnedAt)
                .ToListAsync();

        public async Task<bool> HasBadgeAsync(int donorProfileId, int badgeId)
            => await _context.UserBadges
                .AnyAsync(x => x.DonorProfileId == donorProfileId && x.BadgeId == badgeId);

        public async Task<List<Badge>> GetAllBadgesAsync()
            => await _dbSet.OrderBy(x => x.RequiredDonations).ToListAsync();

        public async Task AddUserBadgeAsync(UserBadge userBadge)
            => await _context.UserBadges.AddAsync(userBadge);
    }
}