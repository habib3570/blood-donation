using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var personal = await _statisticsService.GetPersonalStatsAsync(userId);
            var dashboard = await _statisticsService.GetDashboardStatsAsync();
            var monthly = await _statisticsService.GetMonthlyStatsAsync(userId, DateTime.UtcNow.Year);

            ViewBag.Personal = personal.Data;
            ViewBag.Dashboard = dashboard.Data;
            ViewBag.Monthly = monthly.Data;

            return View();
        }
    }
}