using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class LocationController : Controller
    {
        private readonly ILocationDataService _locationDataService;

        public LocationController(ILocationDataService locationDataService)
        {
            _locationDataService = locationDataService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts()
        {
            var result = await _locationDataService.GetAllDistrictsAsync();
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetUpazilas(int districtId)
        {
            var result = await _locationDataService.GetUpazilasByDistrictIdAsync(districtId);
            return Json(result.Data);
        }
    }
}