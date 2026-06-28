using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Points;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IPointService
    {
        Task<Result<UserPointsDto>> GetUserPointsAsync(string userId);
        Task<Result<List<PointTransactionDto>>> GetTransactionsAsync(
            string userId, int page = 1);
        Task<Result<List<UserPointsDto>>> GetLeaderboardAsync(int count = 10);
    }
}