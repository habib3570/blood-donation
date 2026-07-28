using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.BloodRequest
{
    public class UpdateBloodRequestDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public int UnitsNeeded { get; set; }
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? HospitalAddress { get; set; }
        public string? ContactNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        public RequestPriority Priority { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime? RequiredDate { get; set; }
    }
}