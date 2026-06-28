namespace BloodDonationSystem.Application.DTOs.Donation
{
    public class DonationHistoryDto
    {
        public int Id { get; set; }
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public bool IsEmergency { get; set; }
        public bool CertificateGenerated { get; set; }
        public string? CertificateNumber { get; set; }
    }
}