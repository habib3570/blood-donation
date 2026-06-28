using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class UserLanguagePreference : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = "en";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}