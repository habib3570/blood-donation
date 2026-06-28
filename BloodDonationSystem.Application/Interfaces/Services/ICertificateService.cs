using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Certificate;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ICertificateService
    {
        Task<Result<byte[]>> GenerateCertificateAsync(int donationId);
        Task<Result<CertificateDto>> GetCertificateByDonationIdAsync(int donationId);
        Task<Result<List<CertificateDto>>> GetUserCertificatesAsync(int donorProfileId);
    }
}