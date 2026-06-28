using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.API.v1
{
    public class StatisticsApiController : ApiBaseController
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsApiController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _statisticsService.GetDashboardStatsAsync();
            return ApiResponse(result);
        }

        [HttpGet("heatmap")]
        public async Task<IActionResult> GetHeatmap()
        {
            var result = await _statisticsService.GetDonationHeatmapAsync();
            return ApiResponse(result);
        }
    }
}