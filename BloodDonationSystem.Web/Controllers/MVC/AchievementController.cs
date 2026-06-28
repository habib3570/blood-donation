using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class AchievementController : Controller
    {
        private readonly IAchievementService _achievementService;
        private readonly IBadgeService _badgeService;
        private readonly IDonorService _donorService;

        public AchievementController(
            IAchievementService achievementService,
            IBadgeService badgeService,
            IDonorService donorService)
        {
            _achievementService = achievementService;
            _badgeService = badgeService;
            _donorService = donorService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var donor = await _donorService.GetDonorByUserIdAsync(userId);

            if (donor.IsSuccess && donor.Data != null)
            {
                var achievements = await _achievementService
                    .GetUserAchievementsAsync(donor.Data.DonorProfileId);
                var badges = await _badgeService
                    .GetUserBadgesAsync(donor.Data.DonorProfileId);
                var allAchievements = await _achievementService.GetAllAchievementsAsync();
                var allBadges = await _badgeService.GetAllBadgesAsync();

                ViewBag.UserAchievements = achievements.Data;
                ViewBag.UserBadges = badges.Data;
                ViewBag.AllAchievements = allAchievements.Data;
                ViewBag.AllBadges = allBadges.Data;
                ViewBag.DonorProfile = donor.Data;
            }

            return View();
        }
    }
}