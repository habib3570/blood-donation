using BloodDonationSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.User
{
    public class CreateUserDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public BloodGroup BloodGroup { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string District { get; set; } = string.Empty;

        [Required]
        public string Upazila { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; // Admin, Donor, User
    }

    public class ChangeRoleDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string NewRole { get; set; } = string.Empty; // Admin, Donor, User
    }
}