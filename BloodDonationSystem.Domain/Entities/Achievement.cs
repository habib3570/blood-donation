using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class Achievement : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AchievementType Type { get; set; }
        public string IconUrl { get; set; } = string.Empty;
        public int RequiredCount { get; set; } = 1;
        public int RewardPoints { get; set; } = 0;

        // Navigation Properties
        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}