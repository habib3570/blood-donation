using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IBloodBankRepository : IGenericRepository<BloodBank>
    {
        Task<List<BloodBank>> GetByDistrictAsync(string district);
        Task<List<BloodBank>> GetNearbyBloodBanksAsync(double latitude, double longitude, double radiusKm);
        Task<BloodBank?> GetWithStocksAsync(int id);
        Task<List<BloodBankStock>> GetLowStocksAsync();
        Task<List<BloodBankStock>> GetStocksByBloodGroupAsync(BloodGroup bloodGroup);
        Task UpdateStockAsync(int bloodBankId, BloodGroup bloodGroup, int units);
    }
}