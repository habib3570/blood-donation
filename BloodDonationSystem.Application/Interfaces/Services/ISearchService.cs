using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ISearchService
    {
        Task<Result<List<DonorDto>>> SearchDonorsAsync(DonorFilterDto filter);
        Task<Result> SaveRecentSearchAsync(string userId, DonorFilterDto filter);
        Task<Result<List<string>>> GetRecentSearchesAsync(string userId);
        Task<Result<List<string>>> GetPopularBloodGroupsNeededAsync();
        Task<Result> ClearSearchHistoryAsync(string userId);
    }
}