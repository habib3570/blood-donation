using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class Donation : AuditableEntity
    {
        public int DonorProfileId { get; set; }
        public int? BloodRequestId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public bool IsEmergency { get; set; } = false;
        public bool CertificateGenerated { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
        public BloodRequest? BloodRequest { get; set; }
        public DonationCertificate? Certificate { get; set; }
    }
}