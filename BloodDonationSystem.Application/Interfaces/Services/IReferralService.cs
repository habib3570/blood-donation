using BloodDonationSystem.Application.Common.Models;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IReferralService
    {
        Task<Result<string>> GenerateReferralCodeAsync(string userId);
        Task<Result> UseReferralCodeAsync(string userId, string code);
        Task<Result<int>> GetReferralCountAsync(string userId);
        Task<Result<string>> GetUserReferralCodeAsync(string userId);
    }
}