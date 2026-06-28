using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IFavoriteDonorService
    {
        Task<Result> AddFavoriteAsync(string userId, int donorProfileId);
        Task<Result> RemoveFavoriteAsync(string userId, int donorProfileId);
        Task<Result<List<DonorDto>>> GetFavoriteDonorsAsync(string userId);
        Task<Result<bool>> IsFavoriteAsync(string userId, int donorProfileId);
    }
}