using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonationCertificate : BaseEntity
    {
        public int DonationId { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string? FilePath { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Donation Donation { get; set; } = null!;
    }
}