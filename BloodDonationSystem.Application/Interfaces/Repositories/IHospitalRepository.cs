using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface IHospitalRepository : IGenericRepository<Hospital>
    {
        Task<List<Hospital>> GetByDistrictAsync(string district);
        Task<List<Hospital>> GetNearbyHospitalsAsync(double latitude, double longitude, double radiusKm);
        Task<List<Hospital>> GetHospitalsWithBloodBankAsync();
        Task<Hospital?> GetWithDetailsAsync(int id);
        Task<List<Hospital>> GetOpenHospitalsAsync();
        Task<List<Hospital>> SearchHospitalsAsync(HospitalFilterDto filter);
    }
}