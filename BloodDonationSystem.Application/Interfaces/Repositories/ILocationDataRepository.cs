using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface ILocationDataRepository
    {
        Task<List<District>> GetAllDistrictsAsync();
        Task<List<Upazila>> GetUpazilasByDistrictIdAsync(int districtId);
        Task<District?> GetDistrictByIdAsync(int id);
    }
}