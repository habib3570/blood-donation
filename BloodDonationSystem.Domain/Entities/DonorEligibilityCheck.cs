using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonorEligibilityCheck : BaseEntity
    {
        public int DonorProfileId { get; set; }
        public int? BloodRequestId { get; set; }
        public bool IsEligible { get; set; }
        public string? IneligibilityReason { get; set; }
        public int? DaysUntilEligible { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
    }
}