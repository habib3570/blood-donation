using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Statistics;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;

namespace BloodDonationSystem.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IEmergencyRequestRepository _emergencyRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IBloodBankRepository _bloodBankRepository;
        private readonly IPointRepository _pointRepository;

        public StatisticsService(
            IUserRepository userRepository,
            IDonorRepository donorRepository,
            IDonationRepository donationRepository,
            IBloodRequestRepository bloodRequestRepository,
            IEmergencyRequestRepository emergencyRepository,
            IHospitalRepository hospitalRepository,
            IBloodBankRepository bloodBankRepository,
            IPointRepository pointRepository)
        {
            _userRepository = userRepository;
            _donorRepository = donorRepository;
            _donationRepository = donationRepository;
            _bloodRequestRepository = bloodRequestRepository;
            _emergencyRepository = emergencyRepository;
            _hospitalRepository = hospitalRepository;
            _bloodBankRepository = bloodBankRepository;
            _pointRepository = pointRepository;
        }

        public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            var totalDonors = await _donorRepository.CountAsync();
            var activeDonors = await _donorRepository.CountAsync(x => x.IsAvailable && !x.IsVacationMode);
            var totalDonations = await _donationRepository.GetTotalDonationsCountAsync();
            var emergencyRequests = await _emergencyRepository.CountAsync();
            var pendingRequests = await _bloodRequestRepository.CountAsync(x => x.Status == Domain.Enums.RequestStatus.Pending);
            var totalHospitals = await _hospitalRepository.CountAsync();
            var totalBloodBanks = await _bloodBankRepository.CountAsync();

            var now = DateTime.UtcNow;
            var monthlyData = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var count = await _donationRepository.GetMonthlyDonationsCountAsync(month.Month, month.Year);
                monthlyData.Add(new { month = month.ToString("MMM"), count });
            }

            return Result<DashboardStatsDto>.Success(new DashboardStatsDto
            {
                TotalDonors = totalDonors,
                TotalDonations = totalDonations,
                ActiveDonors = activeDonors,
                EmergencyRequests = (int)emergencyRequests,
                LivesSaved = totalDonations * DonationConstants.LivessavedPerDonation,
                TotalHospitals = totalHospitals,
                TotalBloodBanks = totalBloodBanks,
                PendingRequests = pendingRequests,
                MonthlyDonationChart = monthlyData
            });
        }

        public async Task<Result<PersonalStatsDto>> GetPersonalStatsAsync(string userId)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<PersonalStatsDto>.Success(new PersonalStatsDto());

            var accepted = await _bloodRequestRepository.CountAsync(x => x.DonorId == userId && x.Status == Domain.Enums.RequestStatus.Completed);
            var pending = await _bloodRequestRepository.CountAsync(x => x.RequesterId == userId && x.Status == Domain.Enums.RequestStatus.Pending);
            var rejected = await _bloodRequestRepository.CountAsync(x => x.DonorId == userId && x.Status == Domain.Enums.RequestStatus.Rejected);
            var rank = await _pointRepository.GetUserRankAsync(userId);
            var points = await _pointRepository.GetByUserIdAsync(userId);

            return Result<PersonalStatsDto>.Success(new PersonalStatsDto
            {
                TotalDonations = donor.TotalDonations,
                AcceptedRequests = (int)accepted,
                PendingRequests = (int)pending,
                RejectedRequests = (int)rejected,
                AverageRating = donor.AverageRating,
                TotalPoints = points?.TotalPoints ?? 0,
                LivesSaved = donor.LivesSaved,
                CurrentStreak = donor.DonationStreak,
                Rank = rank,
                Level = donor.Level.ToString()
            });
        }

        public async Task<Result<MonthlyStatsDto>> GetMonthlyStatsAsync(string userId, int year)
        {
            var donor = await _donorRepository.GetByUserIdAsync(userId);
            if (donor == null)
                return Result<MonthlyStatsDto>.Failure("Donor not found.");

            var monthlyData = await _donationRepository.GetMonthlyDonationStatsAsync(donor.Id, year);
            var monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            var data = Enumerable.Range(1, 12).Select(m => new MonthDataDto
            {
                Month = m,
                MonthName = monthNames[m],
                Donations = monthlyData.ContainsKey(m) ? monthlyData[m] : 0
            }).ToList();

            return Result<MonthlyStatsDto>.Success(new MonthlyStatsDto { Year = year, MonthlyData = data });
        }

        public async Task<Result<AdminStatsDto>> GetAdminStatsAsync()
        {
            var totalUsers = await _userRepository.GetTotalUsersCountAsync();
            var totalDonors = await _donorRepository.CountAsync();
            var verifiedDonors = await _donorRepository.CountAsync(x => x.IsVerifiedDonor);
            var blockedUsers = await _userRepository.CountAsync(x => x.IsBlocked);
            var totalRequests = await _bloodRequestRepository.CountAsync();
            var pendingRequests = await _bloodRequestRepository.CountAsync(x => x.Status == Domain.Enums.RequestStatus.Pending);
            var completedRequests = await _bloodRequestRepository.CountAsync(x => x.Status == Domain.Enums.RequestStatus.Completed);
            var emergencyRequests = await _emergencyRepository.CountAsync();
            var totalDonations = await _donationRepository.GetTotalDonationsCountAsync();

            return Result<AdminStatsDto>.Success(new AdminStatsDto
            {
                TotalUsers = totalUsers,
                TotalDonors = (int)totalDonors,
                VerifiedDonors = (int)verifiedDonors,
                BlockedUsers = (int)blockedUsers,
                TotalRequests = (int)totalRequests,
                PendingRequests = (int)pendingRequests,
                CompletedRequests = (int)completedRequests,
                TotalEmergencyRequests = (int)emergencyRequests,
                TotalDonations = totalDonations
            });
        }

        public async Task<Result<List<object>>> GetDonationHeatmapAsync()
        {
            return Result<List<object>>.Success(new List<object>());
        }

        public async Task<Result<List<object>>> GetDistrictWiseStatsAsync()
        {
            return Result<List<object>>.Success(new List<object>());
        }
    }
}