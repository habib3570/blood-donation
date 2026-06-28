using BloodDonationSystem.Application.DTOs;
using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class SuccessStoryController : Controller
    {
        private readonly ISuccessStoryService _successStoryService;

        public SuccessStoryController(ISuccessStoryService successStoryService)
        {
            _successStoryService = successStoryService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _successStoryService.GetApprovedStoriesAsync(1);
            return View(result.Data);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SubmitSuccessStoryDto());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitSuccessStoryDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View(dto);

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "stories");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                dto.ImageUrl = $"/uploads/stories/{fileName}";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _successStoryService.SubmitStoryAsync(userId, dto);

            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Thank you! Your story has been submitted for review."
                : result.Errors.FirstOrDefault();

            return RedirectToAction("Index");
        }
    }
}