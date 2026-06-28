using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminHospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly ILocationDataService _locationDataService;

        public AdminHospitalController(
            IHospitalService hospitalService,
            ILocationDataService locationDataService)
        {
            _hospitalService = hospitalService;
            _locationDataService = locationDataService;
        }

        public async Task<IActionResult> Index(HospitalFilterDto filter)
        {
            var result = await _hospitalService.SearchHospitalsAsync(filter);

            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;
            ViewBag.Filter = filter;

            if (!string.IsNullOrEmpty(filter.District))
            {
                var currentDistrict = districts.Data?.FirstOrDefault(d => d.Name == filter.District);
                if (currentDistrict != null)
                {
                    var upazilas = await _locationDataService.GetUpazilasByDistrictIdAsync(currentDistrict.Id);
                    ViewBag.Upazilas = upazilas.Data;
                }
            }

            return View("~/Views/Admin/Hospitals/Index.cshtml", result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;
            return View("~/Views/Admin/Hospitals/Create.cshtml", new CreateHospitalDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHospitalDto dto)
        {
            if (!ModelState.IsValid)
            {
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View("~/Views/Admin/Hospitals/Create.cshtml", dto);
            }

            var result = await _hospitalService.CreateHospitalAsync(dto);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Hospital created successfully!"
                : result.Errors.FirstOrDefault();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _hospitalService.GetHospitalByIdAsync(id);
            if (!result.IsSuccess) return NotFound();

            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;

            // বর্তমান জেলার DistrictId খুঁজে বের করা (নাম মিলিয়ে)
            var currentDistrict = districts.Data?.FirstOrDefault(d => d.Name == result.Data!.District);
            ViewBag.CurrentDistrictId = currentDistrict?.Id ?? 0;

            var dto = new CreateHospitalDto
            {
                Name = result.Data!.Name,
                District = result.Data.District,
                Upazila = result.Data.Upazila,
                Address = result.Data.Address,
                PhoneNumber = result.Data.PhoneNumber,
                EmergencyNumber = result.Data.EmergencyNumber,
                Latitude = result.Data.Latitude,
                Longitude = result.Data.Longitude,
                IsOpen24Hours = result.Data.IsOpen24Hours,
                OpenTime = result.Data.OpenTime,
                CloseTime = result.Data.CloseTime,
                HasBloodBank = result.Data.HasBloodBank
            };

            ViewBag.HospitalId = id;
            return View("~/Views/Admin/Hospitals/Edit.cshtml", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateHospitalDto dto)
        {
            if (!ModelState.IsValid)
            {
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                ViewBag.HospitalId = id;
                return View("~/Views/Admin/Hospitals/Edit.cshtml", dto);
            }

            var result = await _hospitalService.UpdateHospitalAsync(id, dto);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Hospital updated successfully!"
                : result.Errors.FirstOrDefault();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _hospitalService.GetHospitalByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View("~/Views/Admin/Hospitals/Details.cshtml", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hospitalService.DeleteHospitalAsync(id);
            return Json(new { success = result.IsSuccess, message = result.Errors.FirstOrDefault() ?? result.Message });
        }
    }
}