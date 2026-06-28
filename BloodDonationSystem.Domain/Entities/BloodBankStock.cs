using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodBankStock : AuditableEntity
    {
        public int BloodBankId { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public int UnitsAvailable { get; set; } = 0;
        public int CriticalLevel { get; set; } = 5;
        public bool IsLow => UnitsAvailable <= CriticalLevel;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BloodBank BloodBank { get; set; } = null!;
    }
}