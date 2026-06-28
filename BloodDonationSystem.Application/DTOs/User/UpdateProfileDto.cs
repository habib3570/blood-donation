using BloodDonationSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.User
{
    public class UpdateProfileDto
    {
        [Required] public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        [Required] public BloodGroup BloodGroup { get; set; }
        [Required] public Gender Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string District { get; set; } = string.Empty;
        [Required] public string Upazila { get; set; } = string.Empty;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}