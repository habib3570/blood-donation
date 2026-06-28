namespace BloodDonationSystem.Application.DTOs.Donor
{
    public class UpdateDonorProfileDto
    {
        public string? PreferredArea { get; set; }
        public string? PreferredDistrict { get; set; }
        public string? PreferredUpazila { get; set; }
        public bool IsEmergencyOnly { get; set; }
        public bool IsVacationMode { get; set; }
        public DateTime? VacationEndDate { get; set; }
    }
}