using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class Upazila : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? BengaliName { get; set; }
        public int DistrictId { get; set; }

        public District District { get; set; } = null!;
    }
}