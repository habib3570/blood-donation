using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class UserPoints : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalPoints { get; set; } = 0;
        public int CurrentMonthPoints { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<PointTransaction> Transactions { get; set; } = new List<PointTransaction>();
    }
}