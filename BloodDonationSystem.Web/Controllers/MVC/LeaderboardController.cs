using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class LeaderboardController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IStatisticsService _statisticsService;

        public LeaderboardController(
            IDonorService donorService,
            IStatisticsService statisticsService)
        {
            _donorService = donorService;
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var topDonors = await _donorService.GetTopDonorsAsync(20);
            ViewBag.TopDonors = topDonors.Data;
            return View();
        }
    }
}