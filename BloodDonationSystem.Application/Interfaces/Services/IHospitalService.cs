using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Hospital;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IHospitalService
    {
        Task<Result<List<HospitalDto>>> GetAllHospitalsAsync();
        Task<Result<HospitalDto>> GetHospitalByIdAsync(int id);
        Task<Result<List<HospitalDto>>> GetHospitalsByDistrictAsync(string district);
        Task<Result<List<HospitalDto>>> GetNearbyHospitalsAsync(double latitude, double longitude, double radiusKm);
        Task<Result<HospitalDto>> CreateHospitalAsync(CreateHospitalDto dto);
        Task<Result> UpdateHospitalAsync(int id, CreateHospitalDto dto);
        Task<Result> DeleteHospitalAsync(int id);
        Task<Result<List<HospitalDto>>> GetHospitalsWithBloodBankAsync();
        Task<Result<List<HospitalDto>>> SearchHospitalsAsync(HospitalFilterDto filter);
    }
}