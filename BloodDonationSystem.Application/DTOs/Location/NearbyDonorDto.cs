using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.Location
{
    public class NearbyDonorDto
    {
        public int DonorProfileId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public string BloodGroupDisplay { get; set; } = string.Empty;
        public double DistanceKm { get; set; }
        public bool IsAvailable { get; set; }
        public double AverageRating { get; set; }
        public string? PhoneNumber { get; set; }
    }
}