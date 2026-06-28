using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminUserController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View("~/Views/Admin/Users/Index.cshtml", users.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var result = await _adminService.GetUserDetailsAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View("~/Views/Admin/Users/Details.cshtml", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Block(string id)
        {
            var result = await _adminService.BlockUserAsync(id);
            return Json(new { success = result.IsSuccess, message = result.Errors.FirstOrDefault() ?? result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string id)
        {
            var result = await _adminService.UnblockUserAsync(id);
            return Json(new { success = result.IsSuccess, message = result.Errors.FirstOrDefault() ?? result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyDonor(int donorProfileId)
        {
            var result = await _adminService.VerifyDonorAsync(donorProfileId);
            return Json(new { success = result.IsSuccess, message = result.Errors.FirstOrDefault() ?? result.Message });
        }
    }
}