using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Points;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class PointService : IPointService
    {
        private readonly IPointRepository _pointRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PointService(IPointRepository pointRepository, IUnitOfWork unitOfWork)
        {
            _pointRepository = pointRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserPointsDto>> GetUserPointsAsync(string userId)
        {
            var points = await _pointRepository.GetByUserIdAsync(userId);
            if (points == null)
                return Result<UserPointsDto>.Success(new UserPointsDto
                {
                    UserId = userId,
                    TotalPoints = 0
                });

            return Result<UserPointsDto>.Success(new UserPointsDto
            {
                UserId = userId,
                TotalPoints = points.TotalPoints,
                CurrentMonthPoints = points.CurrentMonthPoints
            });
        }

        public async Task<Result<List<PointTransactionDto>>> GetTransactionsAsync(
            string userId, int page = 1)
        {
            var transactions = await _pointRepository.GetTransactionsAsync(userId, page);
            var dtos = transactions.Select(t => new PointTransactionDto
            {
                Id = t.Id,
                Points = t.Points,
                Reason = t.Reason,
                CreatedAt = t.CreatedAt
            }).ToList();

            return Result<List<PointTransactionDto>>.Success(dtos);
        }

        public async Task<Result<List<UserPointsDto>>> GetLeaderboardAsync(int count = 10)
        {
            var leaderboard = await _pointRepository.GetLeaderboardAsync(count);
            var dtos = leaderboard.Select(p => new UserPointsDto
            {
                UserId = p.UserId,
                TotalPoints = p.TotalPoints,
                CurrentMonthPoints = p.CurrentMonthPoints
            }).ToList();

            return Result<List<UserPointsDto>>.Success(dtos);
        }
    }
}