using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Donation>> GetByDonorProfileIdAsync(int donorProfileId)
            => await _dbSet
                .Include(x => x.Certificate)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.DonationDate)
                .ToListAsync();

        public async Task<Donation?> GetLastDonationAsync(int donorProfileId)
            => await _dbSet
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.DonationDate)
                .FirstOrDefaultAsync();

        public async Task<List<Donation>> GetDonationHistoryAsync(int donorProfileId)
            => await _dbSet
                .Include(x => x.Certificate)
                .Where(x => x.DonorProfileId == donorProfileId)
                .OrderByDescending(x => x.DonationDate)
                .ToListAsync();

        public async Task<int> GetTotalDonationsCountAsync()
            => await _dbSet.CountAsync();

        public async Task<int> GetMonthlyDonationsCountAsync(int month, int year)
            => await _dbSet.CountAsync(x => x.DonationDate.Month == month && x.DonationDate.Year == year);

        public async Task<List<Donation>> GetDonationsByDistrictAsync(string district)
            => await _dbSet
                .Where(x => x.District == district)
                .OrderByDescending(x => x.DonationDate)
                .ToListAsync();

        public async Task<Dictionary<int, int>> GetMonthlyDonationStatsAsync(int donorProfileId, int year)
        {
            var donations = await _dbSet
                .Where(x => x.DonorProfileId == donorProfileId && x.DonationDate.Year == year)
                .GroupBy(x => x.DonationDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();

            return donations.ToDictionary(x => x.Month, x => x.Count);
        }
    }
}