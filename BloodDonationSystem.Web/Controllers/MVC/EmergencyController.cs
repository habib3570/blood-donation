using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.DTOs.Emergency;
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class EmergencyController : Controller
    {
        private readonly IEmergencyService _emergencyService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocationDataService _locationDataService;

        public EmergencyController(
            IEmergencyService emergencyService,
            ICurrentUserService currentUserService,
            ILocationDataService locationDataService)
        {
            _emergencyService = emergencyService;
            _currentUserService = currentUserService;
            _locationDataService = locationDataService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var result = await _emergencyService.GetActiveEmergencyRequestsAsync();
            return View(result.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SOS()
        {
            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;
            return View("~/Views/Emergency/SOS.cshtml");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmergencyRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View("~/Views/Emergency/SOS.cshtml", dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _emergencyService.CreateEmergencyRequestAsync(userId, dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Failed.");
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View("~/Views/Emergency/SOS.cshtml", dto);
            }

            TempData["Success"] = "Emergency request sent! Nearby donors have been notified.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _emergencyService.GetEmergencyRequestByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _emergencyService.AcceptEmergencyRequestAsync(userId, id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Message : result.Errors.FirstOrDefault();
            return RedirectToAction("Details", new { id });
        }
    }
}