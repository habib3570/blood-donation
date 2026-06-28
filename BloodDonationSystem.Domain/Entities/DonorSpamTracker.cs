using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonorSpamTracker : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int EmergencyRequestCountToday { get; set; } = 0;
        public int BloodRequestCountToday { get; set; } = 0;
        public DateTime TrackerDate { get; set; } = DateTime.UtcNow.Date;
        public bool IsBlocked { get; set; } = false;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}