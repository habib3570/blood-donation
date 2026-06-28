using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class CertificateController : Controller
    {
        private readonly ICertificateService _certificateService;
        private readonly IDonorService _donorService;

        public CertificateController(ICertificateService certificateService, IDonorService donorService)
        {
            _certificateService = certificateService;
            _donorService = donorService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var donor = await _donorService.GetDonorByUserIdAsync(userId);
            if (!donor.IsSuccess) return RedirectToAction("Index", "Home");

            var certs = await _certificateService.GetUserCertificatesAsync(donor.Data!.DonorProfileId);
            return View(certs.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int donationId)
        {
            var result = await _certificateService.GenerateCertificateAsync(donationId);
            if (!result.IsSuccess) return NotFound();
            return File(result.Data!, "application/pdf", $"BloodDonation_Certificate_{donationId}.pdf");
        }
    }
}