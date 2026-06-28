using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Location;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<Result> UpdateUserLocationAsync(string userId, double latitude, double longitude);
        Task<Result<List<NearbyDonorDto>>> GetNearbyDonorsAsync(double latitude, double longitude, double radiusKm);
        Task<Result> StartLocationSharingAsync(string donorId, int emergencyRequestId);
        Task<Result> StopLocationSharingAsync(string donorId, int emergencyRequestId);
        Task<Result> UpdateLiveLocationAsync(string donorId, int emergencyRequestId, double latitude, double longitude);
        Task<Result<LocationDto>> GetDonorLiveLocationAsync(string donorId, int emergencyRequestId);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}