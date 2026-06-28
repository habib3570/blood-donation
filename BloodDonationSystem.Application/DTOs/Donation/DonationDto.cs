namespace BloodDonationSystem.Application.DTOs.Donation
{
    public class DonationDto
    {
        public int Id { get; set; }
        public int DonorProfileId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public bool IsEmergency { get; set; }
        public bool CertificateGenerated { get; set; }
    }
}