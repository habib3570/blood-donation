using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class LoginActivity : AuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? Browser { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime LoginAt { get; set; }

        // Navigation Property
        public ApplicationUser User { get; set; } = null!;
    }
}