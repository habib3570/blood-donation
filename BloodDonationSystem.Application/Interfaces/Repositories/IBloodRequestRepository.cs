using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IBloodRequestRepository : IGenericRepository<BloodRequest>
    {
        Task<List<BloodRequest>> GetByRequesterIdAsync(string requesterId);
        Task<List<BloodRequest>> GetByStatusAsync(RequestStatus status);
        Task<List<BloodRequest>> GetByBloodGroupAsync(BloodGroup bloodGroup);
        Task<List<BloodRequest>> GetByDistrictAsync(string district);
        Task<List<BloodRequest>> GetActiveRequestsAsync();
        Task<List<BloodRequest>> GetExpiredRequestsAsync();
        Task<BloodRequest?> GetWithDetailsAsync(int id);
        Task<int> GetTodayRequestCountByUserAsync(string userId);
        Task<List<BloodRequest>> GetRequestsByDonorAsync(string donorId);
        Task UpdateStatusAsync(int requestId, RequestStatus status);

     
        Task<List<BloodRequest>> GetAcceptedRequestsByDonorAsync(string donorId);
    }
}