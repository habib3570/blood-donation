using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class HospitalRepository : GenericRepository<Hospital>, IHospitalRepository
    {
        public HospitalRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Hospital>> GetByDistrictAsync(string district)
            => await _dbSet
                .Where(x => x.District == district && x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();

        public async Task<List<Hospital>> GetNearbyHospitalsAsync(double latitude, double longitude, double radiusKm)
        {
            var hospitals = await _dbSet
                .Where(x => x.IsActive)
                .ToListAsync();

            return hospitals.Where(h =>
            {
                var distance = CalculateDistance(latitude, longitude, h.Latitude, h.Longitude);
                return distance <= radiusKm;
            })
            .OrderBy(h => CalculateDistance(latitude, longitude, h.Latitude, h.Longitude))
            .ToList();
        }

        public async Task<List<Hospital>> GetHospitalsWithBloodBankAsync()
            => await _dbSet
                .Include(x => x.BloodBank)
                .Where(x => x.HasBloodBank && x.IsActive)
                .ToListAsync();

        public async Task<Hospital?> GetWithDetailsAsync(int id)
            => await _dbSet
                .Include(x => x.BloodBank)
                    .ThenInclude(b => b != null ? b.Stocks : null)
                .Include(x => x.EmergencyContacts)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Hospital>> GetOpenHospitalsAsync()
        {
            var now = DateTime.Now;
            return await _dbSet
                .Where(x => x.IsActive && (x.IsOpen24Hours ||
                    (x.OpenTime != null && x.CloseTime != null)))
                .ToListAsync();
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
        public async Task<List<Hospital>> SearchHospitalsAsync(HospitalFilterDto filter)
        {
            var query = _context.Hospitals.Where(h => h.IsActive);

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(h => h.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.District))
                query = query.Where(h => h.District == filter.District);

            if (!string.IsNullOrEmpty(filter.Upazila))
                query = query.Where(h => h.Upazila == filter.Upazila);

            if (filter.HasBloodBank.HasValue)
                query = query.Where(h => h.HasBloodBank == filter.HasBloodBank.Value);

            return await query.OrderBy(h => h.Name).ToListAsync();
        }
    }
}