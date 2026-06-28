using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class MonthlyTopDonor : BaseEntity
    {
        public int DonorProfileId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalDonationsThisMonth { get; set; }
        public int PointsEarnedThisMonth { get; set; }
        public int Rank { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
    }
}