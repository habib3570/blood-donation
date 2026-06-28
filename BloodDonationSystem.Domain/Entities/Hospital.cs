using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class Hospital : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmergencyNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsOpen24Hours { get; set; } = false;
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public bool HasBloodBank { get; set; } = false;
        public string? Website { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public BloodBank? BloodBank { get; set; }
        public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    }
}