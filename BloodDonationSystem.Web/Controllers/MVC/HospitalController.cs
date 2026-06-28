using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly IBloodBankService _bloodBankService;
        private readonly ILocationDataService _locationDataService;

        public HospitalController(
            IHospitalService hospitalService,
            IBloodBankService bloodBankService,
            ILocationDataService locationDataService)
        {
            _hospitalService = hospitalService;
            _bloodBankService = bloodBankService;
            _locationDataService = locationDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? district = null, string? upazila = null)
        {
            List<BloodDonationSystem.Application.DTOs.Hospital.HospitalDto>? hospitals;

            if (!string.IsNullOrEmpty(district))
            {
                var byDistrict = await _hospitalService.GetHospitalsByDistrictAsync(district);
                hospitals = byDistrict.Data;

                if (!string.IsNullOrEmpty(upazila) && hospitals != null)
                {
                    hospitals = hospitals.Where(h => h.Upazila == upazila).ToList();
                }
            }
            else
            {
                var all = await _hospitalService.GetAllHospitalsAsync();
                hospitals = all.Data;
            }

            ViewBag.Hospitals = hospitals;

            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;

            if (!string.IsNullOrEmpty(district))
            {
                var currentDistrict = districts.Data?.FirstOrDefault(d => d.Name == district);
                if (currentDistrict != null)
                {
                    var upazilas = await _locationDataService.GetUpazilasByDistrictIdAsync(currentDistrict.Id);
                    ViewBag.Upazilas = upazilas.Data;
                }
            }

            ViewBag.District = district;
            ViewBag.Upazila = upazila;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _hospitalService.GetHospitalByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> NearbyHospitals()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNearby(
            double lat, double lng, double radius = 5)
        {
            var result = await _hospitalService.GetNearbyHospitalsAsync(
                lat, lng, radius);
            return Json(result.Data);
        }
    }
}