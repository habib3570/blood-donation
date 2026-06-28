using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Auth;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BloodDonationSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILoginActivityRepository _loginActivityRepository;
        private readonly IPointRepository _pointRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            INotificationService notificationService,
            ILoginActivityRepository loginActivityRepository,
            IPointRepository pointRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _notificationService = notificationService;
            _loginActivityRepository = loginActivityRepository;
            _pointRepository = pointRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result.Failure("Email already registered.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                BloodGroup = dto.BloodGroup,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                District = dto.District,
                Upazila = dto.Upazila,
                ReferralCode = GenerateReferralCode(),
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(dto.ReferralCode))
                user.ReferredByUserId = dto.ReferralCode;

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRoleAsync(user, "User");

            await _pointRepository.AddPointsAsync(user.Id, 20, "Profile Created", null);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmUrl = $"{_configuration["AppUrl"]}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmUrl);

            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Registration successful. Please confirm your email.");
        }

        public async Task<Result<string>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                await LogLoginActivity(null, false);
                return Result<string>.Failure("Invalid email or password.");
            }

            if (user.IsBlocked)
                return Result<string>.Failure("Your account has been blocked. Contact admin.");

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return Result<string>.Failure("Account locked out. Try again after 5 minutes.");

            if (!result.Succeeded)
            {
                await LogLoginActivity(user.Id, false);
                return Result<string>.Failure("Invalid email or password.");
            }

            await _userManager.UpdateAsync(user);
            user.LastSeenAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await LogLoginActivity(user.Id, true);
            await _unitOfWork.SaveChangesAsync();

            return Result<string>.Success(user.Id, "Login successful.");
        }

        public async Task<Result> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return Result.Success("Logged out successfully.");
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result.Success("If that email exists, a reset link has been sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{_configuration["AppUrl"]}/Account/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendPasswordResetEmailAsync(user.Email!, resetUrl);

            return Result.Success("Password reset link sent to your email.");
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return Result.Failure("Invalid request.");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(e => e.Description).ToList());

            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Password reset successfully.");
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(e => e.Description).ToList());

            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Password changed successfully.");
        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure("Invalid request.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return Result.Failure("Email confirmation failed.");

            return Result.Success("Email confirmed successfully.");
        }

        public async Task<Result> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Success("If that email exists, confirmation has been resent.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmUrl = $"{_configuration["AppUrl"]}/Account/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmUrl);

            return Result.Success("Confirmation email resent.");
        }

        private async Task LogLoginActivity(string? userId, bool isSuccessful)
        {
            if (userId == null) return;
            var context = _httpContextAccessor.HttpContext;
            var ip = context?.Connection?.RemoteIpAddress?.ToString();
            var userAgent = context?.Request?.Headers["User-Agent"].ToString();
            await _loginActivityRepository.LogActivityAsync(userId, ip, userAgent, userAgent, isSuccessful);
            await _unitOfWork.SaveChangesAsync();
        }

        private static string GenerateReferralCode()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }
    }
}