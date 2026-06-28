using BloodDonationSystem.Application.DTOs.Rating;
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(
            int donorProfileId, CreateRatingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _ratingService.RateDonorAsync(userId, donorProfileId, dto);

            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Rating submitted successfully!"
                : result.Errors.FirstOrDefault();

            return RedirectToAction("Profile", "Donor",
                new { id = donorProfileId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThankYou(int donorProfileId, string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _ratingService.SendThankYouMessageAsync(
                userId, donorProfileId, message);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess
                    ? "Thank you message sent!"
                    : result.Errors.FirstOrDefault()
            });
        }
    }
}