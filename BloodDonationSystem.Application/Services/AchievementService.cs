using AutoMapper;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Achievement;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IMapper _mapper;

        public AchievementService(
            IAchievementRepository achievementRepository,
            IDonorRepository donorRepository,
            IMapper mapper)
        {
            _achievementRepository = achievementRepository;
            _donorRepository = donorRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<AchievementDto>>> GetAllAchievementsAsync()
        {
            var achievements = await _achievementRepository.GetAllAchievementsAsync();
            return Result<List<AchievementDto>>.Success(
                _mapper.Map<List<AchievementDto>>(achievements));
        }

        public async Task<Result<List<UserAchievementDto>>> GetUserAchievementsAsync(
            int donorProfileId)
        {
            var userAchievements = await _achievementRepository
                .GetUserAchievementsAsync(donorProfileId);
            return Result<List<UserAchievementDto>>.Success(
                _mapper.Map<List<UserAchievementDto>>(userAchievements));
        }
    }
}