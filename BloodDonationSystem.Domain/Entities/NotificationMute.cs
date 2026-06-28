using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class NotificationMute : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime MutedUntil { get; set; }
        public DateTime MutedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}