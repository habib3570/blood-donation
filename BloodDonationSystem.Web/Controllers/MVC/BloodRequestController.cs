using BloodDonationSystem.Application.DTOs.BloodRequest;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Application.Services;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class BloodRequestController : Controller
    {
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IDonorService _donorService;
        private readonly ILocationDataService _locationDataService;

        public BloodRequestController(IBloodRequestService bloodRequestService, IDonorService donorService, ILocationDataService locationDataService)
        {
            _bloodRequestService = bloodRequestService;
            _donorService = donorService;
            _locationDataService = locationDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? bloodGroup, string? district, string? upazila, string? priority)
        {
            var allRequests = await _bloodRequestService.GetAllActiveRequestsAsync();
            var filtered = allRequests.Data ?? new List<BloodDonationSystem.Application.DTOs.BloodRequest.BloodRequestDto>();

            if (!string.IsNullOrEmpty(bloodGroup) && Enum.TryParse<BloodDonationSystem.Domain.Enums.BloodGroup>(bloodGroup, out var bg))
                filtered = filtered.Where(r => r.BloodGroup == bg).ToList();

            if (!string.IsNullOrEmpty(district))
                filtered = filtered.Where(r => r.District == district).ToList();

            if (!string.IsNullOrEmpty(upazila))
                filtered = filtered.Where(r => r.Upazila == upazila).ToList();

            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<BloodDonationSystem.Domain.Enums.RequestPriority>(priority, out var pr))
                filtered = filtered.Where(r => r.Priority == pr).ToList();

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

            ViewBag.SelectedDistrict = district;
            ViewBag.SelectedUpazila = upazila;

            return View(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBloodRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View(dto);
            }
            if (!ModelState.IsValid) return View(dto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.CreateRequestAsync(userId, dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Failed.");
                return View(dto);
            }
            TempData["Success"] = "Blood request created successfully!";
            return RedirectToAction("MyRequests");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _bloodRequestService.GetRequestByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.GetRequestsByUserAsync(userId);
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.AcceptRequestAsync(userId, id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Message : result.Errors.FirstOrDefault();
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.RejectRequestAsync(userId, id);
            return Json(new { success = result.IsSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _bloodRequestService.CompleteRequestAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Message : result.Errors.FirstOrDefault();
            return RedirectToAction("Details", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.CancelRequestAsync(userId, id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Message : result.Errors.FirstOrDefault();
            return RedirectToAction("MyRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(int id, string reason)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _bloodRequestService.ReportRequestAsync(userId, id, reason);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
    }
}