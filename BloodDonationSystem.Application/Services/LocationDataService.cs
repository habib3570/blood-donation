using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Location;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;

namespace BloodDonationSystem.Application.Services
{
    public class LocationDataService : ILocationDataService
    {
        private readonly ILocationDataRepository _repository;

        public LocationDataService(ILocationDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<DistrictDto>>> GetAllDistrictsAsync()
        {
            var districts = await _repository.GetAllDistrictsAsync();
            var dtos = districts.Select(d => new DistrictDto
            {
                Id = d.Id,
                Name = d.Name,
                Division = d.Division
            }).ToList();

            return Result<List<DistrictDto>>.Success(dtos);
        }

        public async Task<Result<List<UpazilaDto>>> GetUpazilasByDistrictIdAsync(int districtId)
        {
            var upazilas = await _repository.GetUpazilasByDistrictIdAsync(districtId);
            var dtos = upazilas.Select(u => new UpazilaDto
            {
                Id = u.Id,
                Name = u.Name,
                DistrictId = u.DistrictId
            }).ToList();

            return Result<List<UpazilaDto>>.Success(dtos);
        }
    }
}