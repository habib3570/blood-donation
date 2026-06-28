using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.User
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public string BloodGroupDisplay { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProfileCompletionPercentage { get; set; }
        public DonorInfoDto? DonorInfo { get; set; }
    }

    public class DonorInfoDto
    {
        public int DonorProfileId { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalDonations { get; set; }
        public double AverageRating { get; set; }
        public string Level { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int LivesSaved { get; set; }
        public DateTime? NextEligibleDate { get; set; }
        public int? DaysUntilEligible { get; set; }
    }
}