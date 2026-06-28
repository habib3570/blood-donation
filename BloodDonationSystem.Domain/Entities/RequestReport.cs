using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class RequestReport : BaseEntity
    {
        public int BloodRequestId { get; set; }
        public string ReporterId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool IsReviewed { get; set; } = false;
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BloodRequest BloodRequest { get; set; } = null!;
        public ApplicationUser Reporter { get; set; } = null!;
    }
}