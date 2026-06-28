using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IDonorService _donorService;
        private readonly IRecentSearchRepository _recentSearchRepository;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SearchService(
            IDonorService donorService,
            IRecentSearchRepository recentSearchRepository,
            IBloodRequestRepository bloodRequestRepository,
            IUnitOfWork unitOfWork)
        {
            _donorService = donorService;
            _recentSearchRepository = recentSearchRepository;
            _bloodRequestRepository = bloodRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<DonorDto>>> SearchDonorsAsync(DonorFilterDto filter)
        {
            return await _donorService.SearchDonorsAsync(filter);
        }

        public async Task<Result> SaveRecentSearchAsync(
            string userId, DonorFilterDto filter)
        {
            var searchTerm = $"{filter.BloodGroup} {filter.District} {filter.Upazila}"
                .Trim();

            await _recentSearchRepository.AddSearchAsync(
                userId, searchTerm,
                filter.BloodGroup?.ToString(),
                filter.District,
                filter.Upazila);

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<List<string>>> GetRecentSearchesAsync(string userId)
        {
            var searches = await _recentSearchRepository.GetByUserIdAsync(userId);
            var terms = searches.Select(s => s.SearchTerm).ToList();
            return Result<List<string>>.Success(terms);
        }

        public async Task<Result<List<string>>> GetPopularBloodGroupsNeededAsync()
        {
            var activeRequests = await _bloodRequestRepository.GetActiveRequestsAsync();

            var popular = activeRequests
                .GroupBy(r => r.BloodGroup)
                .OrderByDescending(g => g.Count())
                .Take(4)
                .Select(g => GetBloodGroupDisplay(g.Key))
                .ToList();

            return Result<List<string>>.Success(popular);
        }

        public async Task<Result> ClearSearchHistoryAsync(string userId)
        {
            await _recentSearchRepository.ClearSearchHistoryAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Search history cleared.");
        }

        private static string GetBloodGroupDisplay(Domain.Enums.BloodGroup bg) => bg switch
        {
            Domain.Enums.BloodGroup.APositive => "A+",
            Domain.Enums.BloodGroup.ANegative => "A-",
            Domain.Enums.BloodGroup.BPositive => "B+",
            Domain.Enums.BloodGroup.BNegative => "B-",
            Domain.Enums.BloodGroup.ABPositive => "AB+",
            Domain.Enums.BloodGroup.ABNegative => "AB-",
            Domain.Enums.BloodGroup.OPositive => "O+",
            Domain.Enums.BloodGroup.ONegative => "O-",
            _ => bg.ToString()
        };
    }
}