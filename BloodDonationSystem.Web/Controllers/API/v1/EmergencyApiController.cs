using BloodDonationSystem.Application.DTOs.Emergency;
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.API.v1
{
    public class EmergencyApiController : ApiBaseController
    {
        private readonly IEmergencyService _emergencyService;

        public EmergencyApiController(IEmergencyService emergencyService)
        {
            _emergencyService = emergencyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var result = await _emergencyService.GetActiveEmergencyRequestsAsync();
            return ApiResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _emergencyService.GetEmergencyRequestByIdAsync(id);
            return ApiResponse(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateEmergencyRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _emergencyService.CreateEmergencyRequestAsync(userId, dto);
            return ApiResponse(result);
        }

        [HttpPost("{id}/accept")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _emergencyService.AcceptEmergencyRequestAsync(userId, id);
            return ApiResponse(result);
        }

        [HttpGet("can-send")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CanSend()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _emergencyService.CanSendEmergencyRequestAsync(userId);
            return ApiResponse(result);
        }
    }
}