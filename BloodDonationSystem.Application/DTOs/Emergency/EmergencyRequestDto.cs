using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.Emergency
{
    public class EmergencyRequestDto
    {
        public int Id { get; set; }
        public string RequesterName { get; set; } = string.Empty;
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
        public string ContactNumber { get; set; } = string.Empty;
        public RequestStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NotificationsSent { get; set; }
    }
}