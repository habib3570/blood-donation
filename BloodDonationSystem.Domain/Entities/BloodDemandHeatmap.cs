using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodDemandHeatmap : BaseEntity
    {
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public int RequestCount { get; set; } = 0;
        public int EmergencyCount { get; set; } = 0;
        public double DemandScore { get; set; } = 0;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int Month { get; set; }
        public int Year { get; set; }
    }
}