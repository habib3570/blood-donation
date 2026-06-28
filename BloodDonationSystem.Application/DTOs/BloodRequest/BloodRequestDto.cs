using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.BloodRequest
{
    public class BloodRequestDto
    {
        public int Id { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public string RequesterId { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public string? DonorId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public string BloodGroupDisplay { get; set; } = string.Empty;
        public int UnitsNeeded { get; set; }
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? HospitalAddress { get; set; }
        public double? HospitalLatitude { get; set; }
        public double? HospitalLongitude { get; set; }
        public string? ContactNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public RequestPriority Priority { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}