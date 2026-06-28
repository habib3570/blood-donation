using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class EmergencyRequest : AuditableEntity
    {
        public string RequesterId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public int UnitsNeeded { get; set; } = 1;
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? HospitalAddress { get; set; }
        public double? HospitalLatitude { get; set; }
        public double? HospitalLongitude { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public bool IsActive { get; set; } = true;
        public DateTime ExpiryDate { get; set; }
        public int NotificationsSent { get; set; } = 0;
        public bool IsReported { get; set; } = false;

        // Navigation Properties
        public ApplicationUser Requester { get; set; } = null!;
    }
}