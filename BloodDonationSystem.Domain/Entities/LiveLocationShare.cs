using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class LiveLocationShare : BaseEntity
    {
        public string DonorId { get; set; } = string.Empty;
        public int EmergencyRequestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        // Navigation Properties
        public ApplicationUser Donor { get; set; } = null!;
        public EmergencyRequest EmergencyRequest { get; set; } = null!;
    }
}