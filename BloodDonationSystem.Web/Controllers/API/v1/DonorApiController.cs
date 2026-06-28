using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.API.v1
{
    public class DonorApiController : ApiBaseController
    {
        private readonly IDonorService _donorService;

        public DonorApiController(IDonorService donorService)
        {
            _donorService = donorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopDonors([FromQuery] int count = 10)
        {
            var result = await _donorService.GetTopDonorsAsync(count);
            return ApiResponse(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] DonorFilterDto filter)
        {
            var result = await _donorService.SearchDonorsAsync(filter);
            return ApiResponse(result);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lon,
            [FromQuery] double radius = 5, [FromQuery] BloodGroup? bloodGroup = null)
        {
            var result = await _donorService.GetNearbyDonorsAsync(lat, lon, radius, bloodGroup);
            return ApiResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _donorService.GetDonorByIdAsync(id);
            return ApiResponse(result);
        }

        [HttpPost("toggle-availability")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ToggleAvailability()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _donorService.ToggleAvailabilityAsync(userId);
            return ApiResponse(result);
        }

        [HttpPost("become-donor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> BecomeDonor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _donorService.BecomeDonorAsync(userId);
            return ApiResponse(result);
        }
    }
}