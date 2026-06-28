using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class EmergencyRequestAcceptance : BaseEntity
    {
        public int EmergencyRequestId { get; set; }
        public string DonorId { get; set; } = string.Empty;
        public RequestStatus Status { get; set; } = RequestStatus.Accepted;
        public bool IsLocationSharing { get; set; } = false;
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        public EmergencyRequest EmergencyRequest { get; set; } = null!;
        public ApplicationUser Donor { get; set; } = null!;
    }
}