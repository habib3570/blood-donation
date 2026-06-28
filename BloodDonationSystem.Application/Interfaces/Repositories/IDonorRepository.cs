using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IDonorRepository : IGenericRepository<DonorProfile>
    {
        Task<DonorProfile?> GetByUserIdAsync(string userId);
        Task<DonorProfile?> GetWithDetailsAsync(int id);
        Task<List<DonorProfile>> GetAvailableDonorsAsync();
        Task<List<DonorProfile>> SearchDonorsAsync(BloodGroup bloodGroup, string district, string upazila, Gender? gender = null);
        Task<List<DonorProfile>> GetNearbyDonorsAsync(double latitude, double longitude, double radiusKm, BloodGroup? bloodGroup = null);
        Task<List<DonorProfile>> GetTopDonorsByPointsAsync(int count = 10);
        Task<List<DonorProfile>> GetMonthlyTopDonorsAsync(int month, int year, int count = 10);
        Task<List<DonorProfile>> GetVerifiedDonorsAsync();
        Task UpdateSmartPriorityScoreAsync(int donorProfileId, double score);
        Task<bool> IsEligibleToDonateAsync(int donorProfileId);
        Task<int> GetDaysUntilEligibleAsync(int donorProfileId);
    }
}