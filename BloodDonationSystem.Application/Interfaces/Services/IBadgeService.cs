using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IBadgeService
    {
        Task<Result<List<Badge>>> GetAllBadgesAsync();
        Task<Result<List<UserBadge>>> GetUserBadgesAsync(int donorProfileId);
    }
}