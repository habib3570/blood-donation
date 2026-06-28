namespace BloodDonationSystem.Application.DTOs.Donor
{
    public class DonorAvailabilityDto
    {
        public bool IsAvailable { get; set; }
        public bool IsEligible { get; set; }
        public bool IsVacationMode { get; set; }
        public bool IsEmergencyOnly { get; set; }
        public DateTime? NextEligibleDate { get; set; }
        public int? DaysUntilEligible { get; set; }
        public string? IneligibilityReason { get; set; }
    }
}