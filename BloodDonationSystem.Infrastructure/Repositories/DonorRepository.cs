using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class DonorRepository : GenericRepository<DonorProfile>, IDonorRepository
    {
        public DonorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<DonorProfile?> GetByUserIdAsync(string userId)
            => await _dbSet
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<DonorProfile?> GetWithDetailsAsync(int id)
            => await _dbSet
                .Include(x => x.User)
                .Include(x => x.Donations)
                .Include(x => x.Ratings).ThenInclude(r => r.Rater)
                .Include(x => x.Badges).ThenInclude(b => b.Badge)
                .Include(x => x.Achievements).ThenInclude(a => a.Achievement)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<DonorProfile>> GetAvailableDonorsAsync()
            => await _dbSet
                .Include(x => x.User)
                .Where(x => x.IsAvailable && !x.IsVacationMode && !x.IsDeleted && !x.User.IsBlocked)
                .OrderByDescending(x => x.SmartPriorityScore)
                .ToListAsync();

        public async Task<List<DonorProfile>> SearchDonorsAsync(BloodGroup bloodGroup, string district, string upazila, Gender? gender = null)
        {
            var query = _dbSet
                .Include(x => x.User)
                .Where(x => x.IsAvailable
                    && !x.IsVacationMode
                    && !x.IsDeleted
                    && !x.User.IsBlocked
                    && x.User.BloodGroup == bloodGroup
                    && x.User.District == district);

            if (!string.IsNullOrEmpty(upazila))
                query = query.Where(x => x.User.Upazila == upazila);

            if (gender.HasValue)
                query = query.Where(x => x.User.Gender == gender.Value);

            return await query
                .OrderByDescending(x => x.SmartPriorityScore)
                .ToListAsync();
        }

        public async Task<List<DonorProfile>> GetNearbyDonorsAsync(double latitude, double longitude, double radiusKm, BloodGroup? bloodGroup = null)
        {
            var donors = await _dbSet
                .Include(x => x.User)
                .Where(x => x.IsAvailable
                    && !x.IsVacationMode
                    && !x.IsDeleted
                    && !x.User.IsBlocked
                    && x.User.Latitude.HasValue
                    && x.User.Longitude.HasValue)
                .ToListAsync();

            var nearby = donors.Where(d =>
            {
                var distance = CalculateDistance(latitude, longitude, d.User.Latitude!.Value, d.User.Longitude!.Value);
                return distance <= radiusKm;
            });

            if (bloodGroup.HasValue)
                nearby = nearby.Where(d => d.User.BloodGroup == bloodGroup.Value);

            return nearby.OrderByDescending(x => x.SmartPriorityScore).ToList();
        }

        public async Task<List<DonorProfile>> GetTopDonorsByPointsAsync(int count = 10)
            => await _dbSet
                .Include(x => x.User)
                .Where(x => !x.IsDeleted && !x.User.IsBlocked && x.User.UserPreference != null && x.User.UserPreference.ShowOnLeaderboard)
                .OrderByDescending(x => x.TotalPoints)
                .Take(count)
                .ToListAsync();

        public async Task<List<DonorProfile>> GetMonthlyTopDonorsAsync(int month, int year, int count = 10)
            => await _context.MonthlyTopDonors
                .Include(x => x.DonorProfile).ThenInclude(d => d.User)
                .Where(x => x.Month == month && x.Year == year)
                .OrderBy(x => x.Rank)
                .Take(count)
                .Select(x => x.DonorProfile)
                .ToListAsync();

        public async Task<List<DonorProfile>> GetVerifiedDonorsAsync()
            => await _dbSet
                .Include(x => x.User)
                .Where(x => x.IsVerifiedDonor && !x.IsDeleted)
                .ToListAsync();

        public async Task UpdateSmartPriorityScoreAsync(int donorProfileId, double score)
        {
            var donor = await _dbSet.FindAsync(donorProfileId);
            if (donor != null)
            {
                donor.SmartPriorityScore = score;
                _dbSet.Update(donor);
            }
        }

        public async Task<bool> IsEligibleToDonateAsync(int donorProfileId)
        {
            var donor = await _dbSet.FindAsync(donorProfileId);
            if (donor?.LastDonationDate == null) return true;
            return (DateTime.UtcNow - donor.LastDonationDate.Value).TotalDays >= DonationConstants.MinDaysBetweenDonations;
        }

        public async Task<int> GetDaysUntilEligibleAsync(int donorProfileId)
        {
            var donor = await _dbSet.FindAsync(donorProfileId);
            if (donor?.LastDonationDate == null) return 0;
            var daysSince = (DateTime.UtcNow - donor.LastDonationDate.Value).TotalDays;
            var daysRemaining = DonationConstants.MinDaysBetweenDonations - (int)daysSince;
            return Math.Max(0, daysRemaining);
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}