using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.BloodRequest
{
    public class UpdateRequestStatusDto
    {
        public RequestStatus Status { get; set; }
        public string? Reason { get; set; }
    }
}