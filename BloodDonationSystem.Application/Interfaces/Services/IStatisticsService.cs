using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Statistics;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IStatisticsService
    {
        Task<Result<DashboardStatsDto>> GetDashboardStatsAsync();
        Task<Result<PersonalStatsDto>> GetPersonalStatsAsync(string userId);
        Task<Result<MonthlyStatsDto>> GetMonthlyStatsAsync(string userId, int year);
        Task<Result<AdminStatsDto>> GetAdminStatsAsync();
        Task<Result<List<object>>> GetDonationHeatmapAsync();
        Task<Result<List<object>>> GetDistrictWiseStatsAsync();
    }
}