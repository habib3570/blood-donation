using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class PasswordChangeHistory : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}