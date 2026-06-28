using BloodDonationSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.BloodRequest
{
    public class CreateBloodRequestDto
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
        public string? ContactNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        public RequestPriority Priority { get; set; } = RequestPriority.Normal;
        public DateTime? RequiredDate { get; set; }
        public bool IsEmergency { get; set; } = false;
    }
}