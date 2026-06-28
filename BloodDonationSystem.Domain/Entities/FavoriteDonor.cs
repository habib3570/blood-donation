using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class FavoriteDonor : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public int DonorProfileId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public DonorProfile DonorProfile { get; set; } = null!;
    }
}