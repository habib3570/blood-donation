using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class Badge : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BadgeType Type { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public int RequiredDonations { get; set; } = 0;

        // Navigation Properties
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}