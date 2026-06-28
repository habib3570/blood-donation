using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Location;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ILocationDataService
    {
        Task<Result<List<DistrictDto>>> GetAllDistrictsAsync();
        Task<Result<List<UpazilaDto>>> GetUpazilasByDistrictIdAsync(int districtId);
    }
}