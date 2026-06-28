using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class UserPreference : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public bool EmailNotification { get; set; } = true;
        public bool SmsNotification { get; set; } = true;
        public bool EmergencyAlert { get; set; } = true;
        public bool DonationReminder { get; set; } = true;
        public bool BirthdayWish { get; set; } = true;
        public string Language { get; set; } = "en";
        public bool ShowOnLeaderboard { get; set; } = true;
        public bool ShareLocation { get; set; } = false;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}