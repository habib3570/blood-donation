using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminRequestController : Controller
    {
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IEmergencyService _emergencyService;

        public AdminRequestController(
            IBloodRequestService bloodRequestService,
            IEmergencyService emergencyService)
        {
            _bloodRequestService = bloodRequestService;
            _emergencyService = emergencyService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _bloodRequestService.GetAllActiveRequestsAsync();
            var emergencies = await _emergencyService.GetActiveEmergencyRequestsAsync();

            ViewBag.Emergencies = emergencies.Data;
            return View("~/Views/Admin/Requests/Index.cshtml", requests.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _bloodRequestService.GetRequestByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View("~/Views/Admin/Requests/Details.cshtml", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceComplete(int id)
        {
            var result = await _bloodRequestService.CompleteRequestAsync(id);
            return Json(new { success = result.IsSuccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            return Json(new { success = true });
        }
    }
}