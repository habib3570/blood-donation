using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class RecentSearch : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string SearchTerm { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}