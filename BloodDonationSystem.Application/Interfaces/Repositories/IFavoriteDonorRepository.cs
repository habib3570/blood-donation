using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IFavoriteDonorRepository : IGenericRepository<FavoriteDonor>
    {
        Task<List<FavoriteDonor>> GetFavoritesAsync(string userId);
        Task<bool> IsFavoriteAsync(string userId, int donorProfileId);
        Task RemoveFavoriteAsync(string userId, int donorProfileId);
    }
}