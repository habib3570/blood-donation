using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Application.Services;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class DonorController : Controller
    {
        private readonly IDonorService _donorService;
        private readonly IRatingService _ratingService;
        private readonly IFavoriteDonorService _favoriteDonorService;
        private readonly IBloodCompatibilityService _compatibilityService;
        private readonly IDonationService _donationService;
        private readonly ILocationDataService _locationDataService;

        public DonorController(
            IDonorService donorService,
            IRatingService ratingService,
            IFavoriteDonorService favoriteDonorService,
            IBloodCompatibilityService compatibilityService,
            IDonationService donationService,
            ILocationDataService locationDataService)
        {
            _donorService = donorService;
            _ratingService = ratingService;
            _favoriteDonorService = favoriteDonorService;
            _compatibilityService = compatibilityService;
            _donationService = donationService;
            _locationDataService = locationDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var donors = await _donorService.GetTopDonorsAsync(20);
            return View(donors.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Search(DonorFilterDto filter)
        {
            var result = await _donorService.SearchDonorsAsync(filter);

            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;

            if (!string.IsNullOrEmpty(filter.District))
            {
                var currentDistrict = districts.Data?.FirstOrDefault(d => d.Name == filter.District);
                if (currentDistrict != null)
                {
                    var upazilas = await _locationDataService.GetUpazilasByDistrictIdAsync(currentDistrict.Id);
                    ViewBag.Upazilas = upazilas.Data;
                }
            }

            ViewBag.Donors = result.Data;
            ViewBag.Filter = filter;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var donor = await _donorService.GetDonorByIdAsync(id);
            if (!donor.IsSuccess) return NotFound();

            var ratings = await _ratingService.GetDonorRatingsAsync(id);
            var history = await _donationService.GetDonationHistoryAsync(id);

            ViewBag.Ratings = ratings.Data;
            ViewBag.History = history.Data;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var isFav = await _favoriteDonorService.IsFavoriteAsync(userId, id);
                ViewBag.IsFavorite = isFav.Data;
            }

            return View(donor.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> NearbyDonors()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DonationHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var donor = await _donorService.GetDonorByUserIdAsync(userId);

            if (!donor.IsSuccess || donor.Data == null)
                return RedirectToAction("Profile", "Account");

            var history = await _donationService.GetDonationHistoryAsync(donor.Data.DonorProfileId);
            return View(history.Data);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeDonor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _donorService.BecomeDonorAsync(userId);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Message : result.Errors.FirstOrDefault();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _donorService.ToggleAvailabilityAsync(userId);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavorite(int donorProfileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _favoriteDonorService.AddFavoriteAsync(userId, donorProfileId);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite(int donorProfileId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _favoriteDonorService.RemoveFavoriteAsync(userId, donorProfileId);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }

        [HttpGet]
        [Authorize]
     
        public async Task<IActionResult> MyFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _favoriteDonorService.GetFavoriteDonorsAsync(userId);
            return View(result.Data);
        }

    }
}