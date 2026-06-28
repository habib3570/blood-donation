using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonorRating : AuditableEntity
    {
        public int DonorProfileId { get; set; }
        public string RaterId { get; set; } = string.Empty;
        public int? BloodRequestId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public bool IsThankYouMessage { get; set; } = false;

        // Navigation Properties
        public DonorProfile DonorProfile { get; set; } = null!;
        public ApplicationUser Rater { get; set; } = null!;
    }
}