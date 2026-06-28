using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HospitalService(
            IHospitalRepository hospitalRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _hospitalRepository = hospitalRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<HospitalDto>>> GetAllHospitalsAsync()
        {
            var hospitals = await _hospitalRepository.GetAllAsync();
            return Result<List<HospitalDto>>.Success(
                _mapper.Map<List<HospitalDto>>(hospitals));
        }

        public async Task<Result<HospitalDto>> GetHospitalByIdAsync(int id)
        {
            var hospital = await _hospitalRepository.GetWithDetailsAsync(id);
            if (hospital == null)
                return Result<HospitalDto>.Failure("Hospital not found.");
            return Result<HospitalDto>.Success(_mapper.Map<HospitalDto>(hospital));
        }

        public async Task<Result<List<HospitalDto>>> GetHospitalsByDistrictAsync(
            string district)
        {
            var hospitals = await _hospitalRepository.GetByDistrictAsync(district);
            return Result<List<HospitalDto>>.Success(
                _mapper.Map<List<HospitalDto>>(hospitals));
        }

        public async Task<Result<List<HospitalDto>>> GetNearbyHospitalsAsync(
            double latitude, double longitude, double radiusKm)
        {
            var hospitals = await _hospitalRepository.GetNearbyHospitalsAsync(
                latitude, longitude, radiusKm);
            return Result<List<HospitalDto>>.Success(
                _mapper.Map<List<HospitalDto>>(hospitals));
        }

        public async Task<Result<HospitalDto>> CreateHospitalAsync(CreateHospitalDto dto)
        {
            var hospital = _mapper.Map<Hospital>(dto);
            hospital.IsActive = true;
            hospital.CreatedAt = DateTime.UtcNow;

            await _hospitalRepository.AddAsync(hospital);
            await _unitOfWork.SaveChangesAsync();

            return Result<HospitalDto>.Success(
                _mapper.Map<HospitalDto>(hospital),
                "Hospital created successfully.");
        }

        public async Task<Result> UpdateHospitalAsync(int id, CreateHospitalDto dto)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            if (hospital == null)
                return Result.Failure("Hospital not found.");

            hospital.Name = dto.Name;
            hospital.District = dto.District;
            hospital.Upazila = dto.Upazila;
            hospital.Address = dto.Address;
            hospital.PhoneNumber = dto.PhoneNumber;
            hospital.EmergencyNumber = dto.EmergencyNumber;
            hospital.Latitude = dto.Latitude;
            hospital.Longitude = dto.Longitude;
            hospital.IsOpen24Hours = dto.IsOpen24Hours;
            hospital.OpenTime = dto.OpenTime;
            hospital.CloseTime = dto.CloseTime;
            hospital.HasBloodBank = dto.HasBloodBank;
            hospital.UpdatedAt = DateTime.UtcNow;

            _hospitalRepository.Update(hospital);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Hospital updated successfully.");
        }

        public async Task<Result> DeleteHospitalAsync(int id)
        {
            var hospital = await _hospitalRepository.GetByIdAsync(id);
            if (hospital == null)
                return Result.Failure("Hospital not found.");

            hospital.IsActive = false;
            hospital.UpdatedAt = DateTime.UtcNow;
            _hospitalRepository.Update(hospital);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Hospital deleted.");
        }

        public async Task<Result<List<HospitalDto>>> GetHospitalsWithBloodBankAsync()
        {
            var hospitals = await _hospitalRepository.GetHospitalsWithBloodBankAsync();
            return Result<List<HospitalDto>>.Success(
                _mapper.Map<List<HospitalDto>>(hospitals));
        }
        public async Task<Result<List<HospitalDto>>> SearchHospitalsAsync(HospitalFilterDto filter)
        {
            var hospitals = await _hospitalRepository.SearchHospitalsAsync(filter);
            return Result<List<HospitalDto>>.Success(_mapper.Map<List<HospitalDto>>(hospitals));
        }
    }
}