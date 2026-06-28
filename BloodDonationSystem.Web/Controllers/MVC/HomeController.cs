
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class HomeController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IEmergencyService _emergencyService;
        private readonly IDonorService _donorService;
        private readonly ISuccessStoryService _successStoryService;
        private readonly IBloodCompatibilityService _compatibilityService;

        public HomeController(
            IStatisticsService statisticsService,
            IEmergencyService emergencyService,
            IDonorService donorService,
            ISuccessStoryService successStoryService,
            IBloodCompatibilityService compatibilityService)
        {
            _statisticsService = statisticsService;
            _emergencyService = emergencyService;
            _donorService = donorService;
            _successStoryService = successStoryService;
            _compatibilityService = compatibilityService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _statisticsService.GetDashboardStatsAsync();
            var emergencies = await _emergencyService.GetActiveEmergencyRequestsAsync();
            var topDonors = await _donorService.GetTopDonorsAsync(6);
            var stories = await _successStoryService.GetApprovedStoriesAsync(1);

            ViewBag.Stats = stats.Data;
            ViewBag.Emergencies = emergencies.Data;
            ViewBag.TopDonors = topDonors.Data;
            ViewBag.Stories = stories.Data;

            return View();
        }

        public async Task<IActionResult> About() => View();

        public async Task<IActionResult> HealthTips()
        {
            return View();
        }

        public async Task<IActionResult> FAQ()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}