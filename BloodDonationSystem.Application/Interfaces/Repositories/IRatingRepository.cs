using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IRatingRepository : IGenericRepository<DonorRating>
    {
        Task<List<DonorRating>> GetByDonorProfileIdAsync(int donorProfileId);
        Task<double> GetAverageRatingAsync(int donorProfileId);
        Task<bool> HasRatedAsync(string raterId, int donorProfileId);
        Task<List<DonorRating>> GetRecentRatingsAsync(int donorProfileId, int count = 5);
    }
}