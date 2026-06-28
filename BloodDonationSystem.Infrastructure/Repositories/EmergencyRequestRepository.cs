using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class EmergencyRequestRepository : GenericRepository<EmergencyRequest>, IEmergencyRequestRepository
    {
        public EmergencyRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<EmergencyRequest>> GetActiveEmergencyRequestsAsync()
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.IsActive && x.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<EmergencyRequest>> GetByUserIdAsync(string userId)
            => await _dbSet
                .Where(x => x.RequesterId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<EmergencyRequest>> GetNearbyEmergencyRequestsAsync(double latitude, double longitude, double radiusKm)
        {
            var requests = await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.IsActive && x.ExpiryDate > DateTime.UtcNow
                    && x.HospitalLatitude.HasValue && x.HospitalLongitude.HasValue)
                .ToListAsync();

            return requests.Where(r =>
            {
                var distance = CalculateDistance(latitude, longitude, r.HospitalLatitude!.Value, r.HospitalLongitude!.Value);
                return distance <= radiusKm;
            }).ToList();
        }

        public async Task<int> GetTodayEmergencyCountByUserAsync(string userId)
            => await _dbSet.CountAsync(x => x.RequesterId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date);

        public async Task<List<EmergencyRequest>> GetExpiredRequestsAsync()
            => await _dbSet.Where(x => x.IsActive && x.ExpiryDate < DateTime.UtcNow).ToListAsync();

        public async Task UpdateStatusAsync(int requestId, RequestStatus status)
        {
            var request = await _dbSet.FindAsync(requestId);
            if (request != null)
            {
                request.Status = status;
                if (status != RequestStatus.Pending) request.IsActive = false;
                _dbSet.Update(request);
            }
        }

        public async Task<EmergencyRequest?> GetWithAcceptancesAsync(int id)
            => await _dbSet
                .Include(x => x.Requester)
                .FirstOrDefaultAsync(x => x.Id == id);

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