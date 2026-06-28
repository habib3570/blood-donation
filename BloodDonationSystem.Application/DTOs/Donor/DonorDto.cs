using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.Donor
{
    public class DonorDto
    {
        public int DonorProfileId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public string BloodGroupDisplay { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerifiedDonor { get; set; }
        public bool IsEmergencyOnly { get; set; }
        public int TotalDonations { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
        public DonorLevel Level { get; set; }
        public string LevelDisplay { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int LivesSaved { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? NextEligibleDate { get; set; }
        public double? DistanceKm { get; set; }
        public double SmartPriorityScore { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}