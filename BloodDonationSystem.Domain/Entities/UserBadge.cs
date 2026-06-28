using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class UserBadge : BaseEntity
    {
        public int DonorProfileId { get; set; }
        public int BadgeId { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
        public Badge Badge { get; set; } = null!;
    }
}