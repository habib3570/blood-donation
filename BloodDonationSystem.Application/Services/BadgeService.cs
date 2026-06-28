using AutoMapper;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly IBadgeRepository _badgeRepository;
        private readonly IMapper _mapper;

        public BadgeService(IBadgeRepository badgeRepository, IMapper mapper)
        {
            _badgeRepository = badgeRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<Domain.Entities.Badge>>> GetAllBadgesAsync()
        {
            var badges = await _badgeRepository.GetAllBadgesAsync();
            return Result<List<Domain.Entities.Badge>>.Success(badges);
        }

        public async Task<Result<List<Domain.Entities.UserBadge>>> GetUserBadgesAsync(
            int donorProfileId)
        {
            var userBadges = await _badgeRepository.GetUserBadgesAsync(donorProfileId);
            return Result<List<Domain.Entities.UserBadge>>.Success(userBadges);
        }
    }
}