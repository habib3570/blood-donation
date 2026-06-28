using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC.Admin
{
    [Authorize(Roles = "Admin")]
 
    public class AdminBloodBankController : Controller
    {
        private readonly IBloodBankService _bloodBankService;

        public AdminBloodBankController(IBloodBankService bloodBankService)
        {
            _bloodBankService = bloodBankService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _bloodBankService.GetAllBloodBanksAsync();
            var lowStocks = await _bloodBankService.GetLowStocksAsync();
            ViewBag.LowStocks = lowStocks.Data;
            return View("~/Views/Admin/BloodBank/Index.cshtml", result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _bloodBankService.GetBloodBankByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View("~/Views/Admin/BloodBank/Edit.cshtml", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStock(
            int bloodBankId, BloodGroup bloodGroup, int units)
        {
            var result = await _bloodBankService.UpdateStockAsync(
                bloodBankId, bloodGroup, units);
            return Json(new { success = result.IsSuccess, message = result.Message });
        }
    }
}