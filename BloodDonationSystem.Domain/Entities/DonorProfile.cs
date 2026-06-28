using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class DonorProfile : AuditableEntity
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public bool IsEmergencyOnly { get; set; } = false;
        public bool IsVacationMode { get; set; } = false;
        public DateTime? VacationEndDate { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? NextEligibleDate { get; set; }
        public int TotalDonations { get; set; } = 0;
        public int DonationStreak { get; set; } = 0;
        public double AverageRating { get; set; } = 0;
        public int TotalRatings { get; set; } = 0;
        public DonorLevel Level { get; set; } = DonorLevel.NewDonor;
        public bool IsVerifiedDonor { get; set; } = false;
        public string? PreferredArea { get; set; }
        public string? PreferredDistrict { get; set; }
        public string? PreferredUpazila { get; set; }
        public int TotalPoints { get; set; } = 0;
        public int LivesSaved { get; set; } = 0;
        public double SmartPriorityScore { get; set; } = 0;

        // Navigation Properties
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public ICollection<DonorRating> Ratings { get; set; } = new List<DonorRating>();
        public ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();
        public ICollection<UserAchievement> Achievements { get; set; } = new List<UserAchievement>();
    }
}