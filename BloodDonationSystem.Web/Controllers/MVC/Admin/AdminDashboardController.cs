using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]

    public class AdminDashboardController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IEmergencyService _emergencyService;
        private readonly IBloodBankService _bloodBankService;

        public AdminDashboardController(
            IStatisticsService statisticsService,
            IEmergencyService emergencyService,
            IBloodBankService bloodBankService)
        {
            _statisticsService = statisticsService;
            _emergencyService = emergencyService;
            _bloodBankService = bloodBankService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _statisticsService.GetAdminStatsAsync();
            var lowStocks = await _bloodBankService.GetLowStocksAsync();
            var emergencies = await _emergencyService.GetActiveEmergencyRequestsAsync();

            ViewBag.Stats = stats.Data;
            ViewBag.LowStocks = lowStocks.Data;
            ViewBag.Emergencies = emergencies.Data;

            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}