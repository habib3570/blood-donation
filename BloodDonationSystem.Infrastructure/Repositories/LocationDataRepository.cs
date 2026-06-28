using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class LocationDataRepository : ILocationDataRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<District>> GetAllDistrictsAsync()
        {
            return await _context.Districts
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<Upazila>> GetUpazilasByDistrictIdAsync(int districtId)
        {
            return await _context.Upazilas
                .Where(u => u.DistrictId == districtId)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<District?> GetDistrictByIdAsync(int id)
        {
            return await _context.Districts.FindAsync(id);
        }
    }
}