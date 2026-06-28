using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class GamificationService : IGamificationService
    {
        private readonly IPointRepository _pointRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public GamificationService(
            IPointRepository pointRepository,
            IBadgeRepository badgeRepository,
            IAchievementRepository achievementRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _pointRepository = pointRepository;
            _badgeRepository = badgeRepository;
            _achievementRepository = achievementRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddPointsAsync(string userId, int points, string reason, string? referenceId = null)
        {
            await _pointRepository.AddPointsAsync(userId, points, reason, referenceId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success($"{points} points added.");
        }

        public async Task<Result> CheckAndAwardBadgesAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetWithDetailsAsync(donorProfileId);
            if (donor == null) return Result.Failure("Donor not found.");

            var allBadges = await _badgeRepository.GetAllBadgesAsync();

            foreach (var badge in allBadges)
            {
                if (badge.RequiredDonations > 0 && donor.TotalDonations >= badge.RequiredDonations)
                {
                    var hasBadge = await _badgeRepository.HasBadgeAsync(donorProfileId, badge.Id);
                    if (!hasBadge)
                    {
                        await _badgeRepository.AddUserBadgeAsync(new UserBadge
                        {
                            DonorProfileId = donorProfileId,
                            BadgeId = badge.Id,
                            EarnedAt = DateTime.UtcNow
                        });

                        await _notificationService.SendNotificationAsync(
                            donor.UserId,
                            $"Badge Earned! 🏅",
                            $"Congratulations! You earned the '{badge.Name}' badge!",
                            NotificationType.BadgeEarned);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> CheckAndAwardAchievementsAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetWithDetailsAsync(donorProfileId);
            if (donor == null) return Result.Failure("Donor not found.");

            var allAchievements = await _achievementRepository.GetAllAchievementsAsync();

            foreach (var achievement in allAchievements)
            {
                var hasIt = await _achievementRepository.HasAchievementAsync(donorProfileId, achievement.Id);
                if (hasIt) continue;

                bool earned = achievement.Type switch
                {
                    AchievementType.FirstDonation => donor.TotalDonations >= 1,
                    AchievementType.FiveDonationsClub => donor.TotalDonations >= 5,
                    AchievementType.TenDonationsClub => donor.TotalDonations >= 10,
                    AchievementType.TwentyDonationsClub => donor.TotalDonations >= 20,
                    AchievementType.TenLivesSaved => donor.LivesSaved >= 10,
                    _ => false
                };

                if (earned)
                {
                    await _achievementRepository.AddUserAchievementAsync(new UserAchievement
                    {
                        DonorProfileId = donorProfileId,
                        AchievementId = achievement.Id,
                        EarnedAt = DateTime.UtcNow
                    });

                    await _pointRepository.AddPointsAsync(
                        donor.UserId, achievement.RewardPoints,
                        $"Achievement: {achievement.Title}", achievement.Id.ToString());

                    await _notificationService.SendNotificationAsync(
                        donor.UserId,
                        $"Achievement Unlocked! 🏆",
                        $"You unlocked: {achievement.Title}! +{achievement.RewardPoints} points",
                        NotificationType.AchievementUnlocked);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateDonorLevelAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor == null) return Result.Failure("Donor not found.");

            var points = await _pointRepository.GetByUserIdAsync(donor.UserId);
            var totalPoints = points?.TotalPoints ?? 0;

            donor.Level = totalPoints switch
            {
                >= PointConstants.Level4RequiredPoints => DonorLevel.LifeSaver,
                >= PointConstants.Level3RequiredPoints => DonorLevel.Hero,
                >= PointConstants.Level2RequiredPoints => DonorLevel.Helper,
                _ => DonorLevel.NewDonor
            };

            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateStreakAsync(int donorProfileId)
        {
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor == null) return Result.Failure("Donor not found.");

            donor.DonationStreak++;
            _donorRepository.Update(donor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateMonthlyTopDonorsAsync()
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<int>> GetUserRankAsync(string userId)
        {
            var rank = await _pointRepository.GetUserRankAsync(userId);
            return Result<int>.Success(rank);
        }

        public async Task<Result<int>> GetUserPointsAsync(string userId)
        {
            var points = await _pointRepository.GetByUserIdAsync(userId);
            return Result<int>.Success(points?.TotalPoints ?? 0);
        }
    }
}