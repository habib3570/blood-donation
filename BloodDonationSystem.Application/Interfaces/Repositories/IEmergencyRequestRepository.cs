using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IEmergencyRequestRepository : IGenericRepository<EmergencyRequest>
    {
        Task<List<EmergencyRequest>> GetActiveEmergencyRequestsAsync();
        Task<List<EmergencyRequest>> GetByUserIdAsync(string userId);
        Task<List<EmergencyRequest>> GetNearbyEmergencyRequestsAsync(double latitude, double longitude, double radiusKm);
        Task<int> GetTodayEmergencyCountByUserAsync(string userId);
        Task<List<EmergencyRequest>> GetExpiredRequestsAsync();
        Task UpdateStatusAsync(int requestId, RequestStatus status);
        Task<EmergencyRequest?> GetWithAcceptancesAsync(int id);
    }
}