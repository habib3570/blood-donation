using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class District : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? BengaliName { get; set; }
        public string Division { get; set; } = string.Empty;

        public ICollection<Upazila> Upazilas { get; set; } = new List<Upazila>();
    }
}