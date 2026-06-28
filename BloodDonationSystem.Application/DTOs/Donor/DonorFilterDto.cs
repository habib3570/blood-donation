using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.Donor
{
    public class DonorFilterDto
    {
        public BloodGroup? BloodGroup { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public Gender? Gender { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusKm { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsVerified { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}