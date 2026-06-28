using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class SuccessStory : AuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsApproved { get; set; } = false;
        public int ViewCount { get; set; } = 0;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
    }
}