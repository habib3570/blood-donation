using BloodDonationSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public BloodGroup BloodGroup { get; set; }
        [Required] public Gender Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string District { get; set; } = string.Empty;
        [Required] public string Upazila { get; set; } = string.Empty;
        public string? ReferralCode { get; set; }
    }
}