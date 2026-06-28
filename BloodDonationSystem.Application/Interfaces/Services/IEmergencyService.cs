using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Emergency;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IEmergencyService
    {
        Task<Result<EmergencyRequestDto>> CreateEmergencyRequestAsync(string userId, CreateEmergencyRequestDto dto);
        Task<Result<List<EmergencyRequestDto>>> GetActiveEmergencyRequestsAsync();
        Task<Result<EmergencyRequestDto>> GetEmergencyRequestByIdAsync(int id);
        Task<Result> AcceptEmergencyRequestAsync(string donorId, int requestId);
        Task<Result> CompleteEmergencyRequestAsync(int requestId);
        Task<Result<bool>> CanSendEmergencyRequestAsync(string userId);
        Task<Result> NotifyNearbyDonorsAsync(int emergencyRequestId);
        Task<Result> ExpireOldEmergencyRequestsAsync();
    }
}