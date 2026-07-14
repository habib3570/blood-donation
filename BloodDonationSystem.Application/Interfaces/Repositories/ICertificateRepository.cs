using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface ICertificateRepository
    {
        Task<Donation?> GetDonationWithDetailsAsync(int donationId);
        Task<DonationCertificate?> GetByDonationIdAsync(int donationId);
        Task<List<DonationCertificate>> GetByDonorProfileIdAsync(int donorProfileId);
        Task AddAsync(DonationCertificate certificate);
    }
}