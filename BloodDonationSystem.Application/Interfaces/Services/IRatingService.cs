using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Rating;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IRatingService
    {
        Task<Result> RateDonorAsync(string raterId, int donorProfileId, CreateRatingDto dto);
        Task<Result<List<RatingDto>>> GetDonorRatingsAsync(int donorProfileId);
        Task<Result<double>> GetAverageRatingAsync(int donorProfileId);
        Task<Result> SendThankYouMessageAsync(string senderId, int donorProfileId, string message);
        Task<Result<bool>> HasRatedAsync(string raterId, int donorProfileId);
    }
}