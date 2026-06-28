namespace BloodDonationSystem.Application.DTOs.Certificate
{
    public class CertificateDto
    {
        public int Id { get; set; }
        public int DonationId { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public string DonorName { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}