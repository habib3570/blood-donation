using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class LoginActivityRepository : GenericRepository<LoginActivity>, ILoginActivityRepository
    {
        public LoginActivityRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<LoginActivity>> GetByUserIdAsync(string userId, int count = 10)
            => await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.LoginAt)
                .Take(count)
                .ToListAsync();

        public async Task<LoginActivity?> GetLastLoginAsync(string userId)
            => await _dbSet
                .Where(x => x.UserId == userId && x.IsSuccessful)
                .OrderByDescending(x => x.LoginAt)
                .FirstOrDefaultAsync();

        public async Task LogActivityAsync(string userId, string? ipAddress, string? deviceInfo, string? browser, bool isSuccessful)
        {
            await _dbSet.AddAsync(new LoginActivity
            {
                UserId = userId,
                IpAddress = ipAddress,
                DeviceInfo = deviceInfo,
                Browser = browser,
                IsSuccessful = isSuccessful,
                LoginAt = DateTime.UtcNow
            });
        }
    }
}