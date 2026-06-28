using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class BloodBankRepository : GenericRepository<BloodBank>, IBloodBankRepository
    {
        public BloodBankRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<BloodBank>> GetByDistrictAsync(string district)
            => await _dbSet
                .Include(x => x.Stocks)
                .Where(x => x.District == district && x.IsActive)
                .ToListAsync();

        public async Task<List<BloodBank>> GetNearbyBloodBanksAsync(double latitude, double longitude, double radiusKm)
        {
            var banks = await _dbSet
                .Include(x => x.Stocks)
                .Where(x => x.IsActive)
                .ToListAsync();

            return banks.Where(b =>
            {
                var distance = CalculateDistance(latitude, longitude, b.Latitude, b.Longitude);
                return distance <= radiusKm;
            })
            .OrderBy(b => CalculateDistance(latitude, longitude, b.Latitude, b.Longitude))
            .ToList();
        }

        public async Task<BloodBank?> GetWithStocksAsync(int id)
            => await _dbSet
                .Include(x => x.Stocks)
                .Include(x => x.Hospital)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<BloodBankStock>> GetLowStocksAsync()
            => await _context.BloodBankStocks
                .Include(x => x.BloodBank)
                .Where(x => x.UnitsAvailable <= x.CriticalLevel)
                .ToListAsync();

        public async Task<List<BloodBankStock>> GetStocksByBloodGroupAsync(BloodGroup bloodGroup)
            => await _context.BloodBankStocks
                .Include(x => x.BloodBank)
                .Where(x => x.BloodGroup == bloodGroup && x.BloodBank.IsActive)
                .ToListAsync();

        public async Task UpdateStockAsync(int bloodBankId, BloodGroup bloodGroup, int units)
        {
            var stock = await _context.BloodBankStocks
                .FirstOrDefaultAsync(x => x.BloodBankId == bloodBankId && x.BloodGroup == bloodGroup);

            if (stock == null)
            {
                await _context.BloodBankStocks.AddAsync(new BloodBankStock
                {
                    BloodBankId = bloodBankId,
                    BloodGroup = bloodGroup,
                    UnitsAvailable = units,
                    LastUpdated = DateTime.UtcNow
                });
            }
            else
            {
                stock.UnitsAvailable = units;
                stock.LastUpdated = DateTime.UtcNow;
            }
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}