using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.User;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IAdminService
    {
        Task<Result<List<UserProfileDto>>> GetAllUsersAsync(int page = 1, int pageSize = 20);
        Task<Result> BlockUserAsync(string userId);
        Task<Result> UnblockUserAsync(string userId);
        Task<Result> VerifyDonorAsync(int donorProfileId);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result<UserProfileDto>> GetUserDetailsAsync(string userId);
        Task<Result<List<object>>> GetSystemReportsAsync();
        Task<Result> ReviewRequestReportAsync(int reportId, bool isFake);
        Task<Result<List<object>>> GetMostActiveDistrictsAsync();
      
    }
}