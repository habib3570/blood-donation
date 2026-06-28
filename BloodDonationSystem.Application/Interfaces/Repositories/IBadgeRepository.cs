using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IBadgeRepository : IGenericRepository<Badge>
    {
        Task<List<UserBadge>> GetUserBadgesAsync(int donorProfileId);
        Task<bool> HasBadgeAsync(int donorProfileId, int badgeId);
        Task<List<Badge>> GetAllBadgesAsync();
        Task AddUserBadgeAsync(UserBadge userBadge);
    }
}