using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]

    public class AdminReportController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IAdminService _adminService;

        public AdminReportController(
            IStatisticsService statisticsService,
            IAdminService adminService)
        {
            _statisticsService = statisticsService;
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _statisticsService.GetAdminStatsAsync();
            var districts = await _adminService.GetMostActiveDistrictsAsync();

            ViewBag.Stats = stats.Data;
            ViewBag.Districts = districts.Data;

            return View("~/Views/Admin/Reports/Index.cshtml");
        }
    }
}