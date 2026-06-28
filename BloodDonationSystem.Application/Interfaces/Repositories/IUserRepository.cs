using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByUserNameAsync(string userName);
        Task<ApplicationUser?> GetWithProfileAsync(string userId);
        Task<List<ApplicationUser>> GetAllWithProfilesAsync();
        Task<List<ApplicationUser>> GetBlockedUsersAsync();
        Task<int> GetTotalUsersCountAsync();
        Task<List<ApplicationUser>> GetUsersByDistrictAsync(string district);
        Task UpdateLastSeenAsync(string userId);
    }
}