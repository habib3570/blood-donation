using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.Achievement
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AchievementType Type { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public int RequiredCount { get; set; }
        public int RewardPoints { get; set; }
    }

    public class UserAchievementDto
    {
        public int Id { get; set; }
        public int DonorProfileId { get; set; }
        public AchievementDto Achievement { get; set; } = null!;
        public DateTime EarnedAt { get; set; }
    }
}