using BloodDonationSystem.Domain.Entities;
namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IDonationRepository : IGenericRepository<Donation>
    {
        Task<List<Donation>> GetByDonorProfileIdAsync(int donorProfileId);
        Task<Donation?> GetLastDonationAsync(int donorProfileId);
        Task<List<Donation>> GetDonationHistoryAsync(int donorProfileId);
        Task<int> GetTotalDonationsCountAsync();
        Task<int> GetMonthlyDonationsCountAsync(int month, int year);
        Task<List<Donation>> GetDonationsByDistrictAsync(string district);
        Task<Dictionary<int, int>> GetMonthlyDonationStatsAsync(int donorProfileId, int year);
        Task<int> GetDonationsCountSinceAsync(DateTime since);
    }
}