using BloodDonationSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.Emergency
{
    public class CreateEmergencyRequestDto
    {
        [Required] public string PatientName { get; set; } = string.Empty;
        [Required] public BloodGroup BloodGroup { get; set; }
        [Range(1, 10)] public int UnitsNeeded { get; set; } = 1;
        [Required] public string HospitalName { get; set; } = string.Empty;
        [Required] public string District { get; set; } = string.Empty;
        [Required] public string Upazila { get; set; } = string.Empty;
        public string? HospitalAddress { get; set; }
        public double? HospitalLatitude { get; set; }
        public double? HospitalLongitude { get; set; }
        [Required] public string ContactNumber { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
    }
}