using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface ILoginActivityRepository : IGenericRepository<LoginActivity>
    {
        Task<List<LoginActivity>> GetByUserIdAsync(string userId, int count = 10);
        Task<LoginActivity?> GetLastLoginAsync(string userId);
        Task LogActivityAsync(string userId, string? ipAddress, string? deviceInfo, string? browser, bool isSuccessful);
    }
}