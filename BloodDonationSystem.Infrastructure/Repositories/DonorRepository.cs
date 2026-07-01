using BloodDonationSystem.Application.Interfaces.Repositories;
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
            => await _context.DonorProfiles
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

        public async Task<DonorProfile?> GetWithDetailsAsync(int id)
            => await _context.DonorProfiles
                .Include(d => d.User)
                .Include(d => d.Donations)
                .Include(d => d.Badges)
                .Include(d => d.Achievements)
                .FirstOrDefaultAsync(d => d.Id == id);

        public async Task<List<DonorProfile>> GetAvailableDonorsAsync()
            => await _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => d.IsAvailable
                         && d.User != null
                         && !d.User.IsBlocked)
                .OrderByDescending(d => d.SmartPriorityScore)
                .ToListAsync();

        public async Task<List<DonorProfile>> SearchDonorsAsync(
            BloodGroup bloodGroup, string district, string upazila, Gender? gender = null)
        {
            var query = _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => d.User != null
                         && d.User.BloodGroup == bloodGroup
                         && d.IsAvailable
                         && !d.User.IsBlocked);

            if (!string.IsNullOrEmpty(district))
                query = query.Where(d => d.PreferredDistrict == district);

            if (!string.IsNullOrEmpty(upazila))
                query = query.Where(d => d.PreferredUpazila == upazila);

            if (gender.HasValue)
                query = query.Where(d => d.User!.Gender == gender.Value);

            return await query
                .OrderByDescending(d => d.SmartPriorityScore)
                .ToListAsync();
        }

        public async Task<List<DonorProfile>> GetNearbyDonorsAsync(
            double latitude, double longitude, double radiusKm, BloodGroup? bloodGroup = null)
        {
            var query = _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => d.IsAvailable
                         && d.User != null
                         && !d.User.IsBlocked);

            if (bloodGroup.HasValue)
                query = query.Where(d => d.User!.BloodGroup == bloodGroup.Value);

            // Latitude/Longitude নেই তাই সব available donor return করছি
            var donors = await query
                .OrderByDescending(d => d.SmartPriorityScore)
                .ToListAsync();

            return donors;
        }

        public async Task<List<DonorProfile>> GetTopDonorsByPointsAsync(int count)
           => await _dbSet
        .Include(x => x.User)
        .Where(x => x.IsAvailable
                 && !x.IsVacationMode
                 && !x.User.IsBlocked)
        .OrderByDescending(x => x.SmartPriorityScore)
        .Take(count)
        .ToListAsync();

        public async Task<List<DonorProfile>> GetMonthlyTopDonorsAsync(int month, int year, int count = 10)
        {
            var donorIds = await _context.Donations
                .Where(d => d.DonationDate.Month == month && d.DonationDate.Year == year)
                .GroupBy(d => d.DonorProfileId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToListAsync();

            return await _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => donorIds.Contains(d.Id))
                .ToListAsync();
        }

        public async Task<List<DonorProfile>> GetVerifiedDonorsAsync()
            => await _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => d.IsVerifiedDonor   // ✅ IsVerifiedDonor ব্যবহার করা হয়েছে
                         && d.User != null
                         && !d.User.IsBlocked)
                .OrderByDescending(d => d.SmartPriorityScore)
                .ToListAsync();

        public async Task UpdateSmartPriorityScoreAsync(int donorProfileId, double score)
        {
            var donor = await _context.DonorProfiles.FindAsync(donorProfileId);
            if (donor != null)
            {
                donor.SmartPriorityScore = score;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEligibleToDonateAsync(int donorProfileId)
        {
            var donor = await _context.DonorProfiles.FindAsync(donorProfileId);
            if (donor == null) return false;
            if (donor.NextEligibleDate == null) return true;
            return DateTime.UtcNow >= donor.NextEligibleDate;
        }

        public async Task<int> GetDaysUntilEligibleAsync(int donorProfileId)
        {
            var donor = await _context.DonorProfiles.FindAsync(donorProfileId);
            if (donor == null || donor.NextEligibleDate == null) return 0;
            var days = (donor.NextEligibleDate.Value - DateTime.UtcNow).Days;
            return days < 0 ? 0 : days;
        }

        public async Task<List<DonorProfile>> GetDonorsByBloodGroupAsync(BloodGroup bloodGroup)
            => await _context.DonorProfiles
                .Include(d => d.User)
                .Where(d => d.User != null
                         && d.User.BloodGroup == bloodGroup
                         && d.IsAvailable
                         && !d.User.IsBlocked)
                .ToListAsync();
    }
}
