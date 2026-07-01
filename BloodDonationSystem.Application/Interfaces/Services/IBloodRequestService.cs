using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.BloodRequest;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IBloodRequestService
    {
        Task<Result<BloodRequestDto>> CreateRequestAsync(string userId, CreateBloodRequestDto dto);
        Task<Result<BloodRequestDto>> GetRequestByIdAsync(int id);
        Task<Result<List<BloodRequestDto>>> GetAllActiveRequestsAsync();
        Task<Result<List<BloodRequestDto>>> GetRequestsByUserAsync(string userId);
        Task<Result> AcceptRequestAsync(string donorId, int requestId);
        Task<Result> RejectRequestAsync(string donorId, int requestId);
        Task<Result> CompleteRequestAsync(int requestId);
        Task<Result> CancelRequestAsync(string userId, int requestId);
        Task<Result> ReportRequestAsync(string userId, int requestId, string reason);
        Task<Result> ExpireOldRequestsAsync();
        Task<Result<int>> GetTodayRequestCountAsync(string userId);

     
        Task<Result<List<BloodRequestDto>>> GetAcceptedRequestsByDonorAsync(string donorId);

    
        Task<Result> CancelAcceptedRequestByDonorAsync(string donorId, int requestId);
    }
}