using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodRequest : AuditableEntity
    {
        public string RequesterId { get; set; } = string.Empty;
        public string? DonorId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public BloodGroup BloodGroup { get; set; }
        public int UnitsNeeded { get; set; } = 1;
        public string HospitalName { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? HospitalAddress { get; set; }
        public double? HospitalLatitude { get; set; }
        public double? HospitalLongitude { get; set; }
        public string? ContactNumber { get; set; }
        public string? AdditionalInfo { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public RequestPriority Priority { get; set; } = RequestPriority.Normal;
        public DateTime? RequiredDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsEmergency { get; set; } = false;
        public bool IsReported { get; set; } = false;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        public ApplicationUser Requester { get; set; } = null!;
        public ApplicationUser? Donor { get; set; }
        public ICollection<RequestReport> Reports { get; set; } = new List<RequestReport>();
    }
}