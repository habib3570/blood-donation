using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class BloodBankController : Controller
    {
        private readonly IBloodBankService _bloodBankService;

        public BloodBankController(IBloodBankService bloodBankService)
        {
            _bloodBankService = bloodBankService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _bloodBankService.GetAllBloodBanksAsync();
            var lowStocks = await _bloodBankService.GetLowStocksAsync();
            ViewBag.LowStocks = lowStocks.Data;
            return View(result.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _bloodBankService.GetBloodBankByIdAsync(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetByBloodGroup(BloodGroup bloodGroup)
        {
            var result = await _bloodBankService.GetStocksByBloodGroupAsync(bloodGroup);
            return Json(result.Data);
        }
    }
}