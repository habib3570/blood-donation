using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class UserAchievement : BaseEntity
    {
        public int DonorProfileId { get; set; }
        public int AchievementId { get; set; }
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
        public Achievement Achievement { get; set; } = null!;
    }
}