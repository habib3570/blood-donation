using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly ApplicationDbContext _context;
        public CertificateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Donation?> GetDonationWithDetailsAsync(int donationId)
            => await _context.Donations
                .Include(d => d.DonorProfile)
                    .ThenInclude(dp => dp.User)
                .Include(d => d.Certificate)
                .FirstOrDefaultAsync(d => d.Id == donationId);

        public async Task<DonationCertificate?> GetByDonationIdAsync(int donationId)
            => await _context.DonationCertificates
                .FirstOrDefaultAsync(c => c.DonationId == donationId);

        public async Task<List<DonationCertificate>> GetByDonorProfileIdAsync(int donorProfileId)
            => await _context.DonationCertificates
                .Include(c => c.Donation)
                .Where(c => c.Donation.DonorProfileId == donorProfileId)
                .OrderByDescending(c => c.GeneratedAt)
                .ToListAsync();

        public async Task AddAsync(DonationCertificate certificate)
            => await _context.DonationCertificates.AddAsync(certificate);
    }
}