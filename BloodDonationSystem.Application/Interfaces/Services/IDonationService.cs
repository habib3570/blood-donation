using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donation;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IDonationService
    {
        Task<Result> RecordDonationAsync(int donorProfileId, int? bloodRequestId, string hospitalName, string district, bool isEmergency = false);
        Task<Result<List<DonationHistoryDto>>> GetDonationHistoryAsync(int donorProfileId);
        Task<Result<DonationDto>> GetLastDonationAsync(int donorProfileId);
        Task<Result<int>> GetTotalDonationsAsync();
        Task<Result<Dictionary<int, int>>> GetMonthlyStatsAsync(int donorProfileId, int year);
        Task<Result<int>> GetDaysUntilNextDonationAsync(int donorProfileId);
        Task<Result> UpdateDonorLevelAsync(int donorProfileId);
        Task<Result> UpdateDonationStreakAsync(int donorProfileId);
    }
}