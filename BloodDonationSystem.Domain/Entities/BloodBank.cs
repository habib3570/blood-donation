using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodBank : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? HospitalId { get; set; }
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Hospital? Hospital { get; set; }
        public ICollection<BloodBankStock> Stocks { get; set; } = new List<BloodBankStock>();
    }
}