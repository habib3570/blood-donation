using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.DTOs.Auth;
using BloodDonationSystem.Application.DTOs.User;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocationDataService _locationDataService;

        public AccountController(
            IAuthService authService,
            IUserProfileService userProfileService,
            ICurrentUserService currentUserService,
            ILocationDataService locationDataService)
        {
            _authService = authService;
            _userProfileService = userProfileService;
            _currentUserService = currentUserService;
            _locationDataService = locationDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (_currentUserService.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var districts = await _locationDataService.GetAllDistrictsAsync();
            ViewBag.Districts = districts.Data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View(dto);
            }

            var result = await _authService.RegisterAsync(dto);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);

                var districts = await _locationDataService.GetAllDistrictsAsync();
                ViewBag.Districts = districts.Data;
                return View(dto);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_currentUserService.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _authService.LoginAsync(dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Login failed.");
                return View(dto);
            }

            TempData["Success"] = "Welcome back!";
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = await _authService.ForgotPasswordAsync(dto);
            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordDto { UserId = userId, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Reset failed.");
                return View(dto);
            }
            TempData["Success"] = "Password reset successfully. Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _authService.ChangePasswordAsync(userId, dto);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Change failed.");
                return View(dto);
            }
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Email confirmed!" : "Confirmation failed.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _userProfileService.GetProfileAsync(userId);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _userProfileService.GetProfileAsync(userId);
            if (!profile.IsSuccess) return NotFound();

            var dto = new UpdateProfileDto
            {
                FullName = profile.Data!.FullName,
                BloodGroup = profile.Data.BloodGroup,
                Gender = profile.Data.Gender,
                DateOfBirth = profile.Data.DateOfBirth,
                District = profile.Data.District,
                Upazila = profile.Data.Upazila
            };

            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _userProfileService.UpdateProfileAsync(userId, dto);

            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Profile updated successfully!"
                : result.Errors.FirstOrDefault();

            return RedirectToAction("Profile");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LoginActivity()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _userProfileService.GetLoginActivityAsync(userId);
            return View(result.Data);
        }

        public IActionResult AccessDenied() => View();
    }
}