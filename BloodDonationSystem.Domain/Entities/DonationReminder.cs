using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonationReminder : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime ReminderDate { get; set; }
        public bool IsSent { get; set; } = false;
        public DateTime? SentAt { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}