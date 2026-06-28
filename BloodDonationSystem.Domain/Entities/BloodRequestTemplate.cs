using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodRequestTemplate : AuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public int UnitsNeeded { get; set; } = 1;
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? AdditionalInfo { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}