using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IDonorService
    {
        Task<Result<DonorDto>> GetDonorByUserIdAsync(string userId);
        Task<Result<DonorDto>> GetDonorByIdAsync(int id);
        Task<Result> BecomeDonorAsync(string userId);
        Task<Result> UpdateDonorProfileAsync(string userId, UpdateDonorProfileDto dto);
        Task<Result> ToggleAvailabilityAsync(string userId);
        Task<Result> SetVacationModeAsync(string userId, DateTime? endDate);
        Task<Result> SetEmergencyOnlyModeAsync(string userId, bool emergencyOnly);
        Task<Result<List<DonorDto>>> SearchDonorsAsync(DonorFilterDto filter);
        Task<Result<List<DonorDto>>> GetNearbyDonorsAsync(double latitude, double longitude, double radiusKm, BloodGroup? bloodGroup = null);
        Task<Result<List<DonorDto>>> GetTopDonorsAsync(int count = 10);
        Task<Result<DonorAvailabilityDto>> CheckAvailabilityAsync(int donorProfileId);
        Task<Result> UpdateSmartPriorityScoresAsync();
        Task<Result> VerifyDonorAsync(int donorProfileId);
    }
}