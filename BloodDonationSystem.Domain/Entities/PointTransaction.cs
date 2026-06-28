using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class PointTransaction : BaseEntity
    {
        public int UserPointsId { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public UserPoints UserPoints { get; set; } = null!;
    }
}