using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class EmergencyContact : BaseEntity
    {
        public int HospitalId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Hospital Hospital { get; set; } = null!;
    }
}