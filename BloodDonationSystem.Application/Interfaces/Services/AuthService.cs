using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Auth;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterDto dto);
        Task<Result<string>> LoginAsync(LoginDto dto);
        Task<Result> LogoutAsync();
        Task<Result> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task<Result> ResendConfirmationEmailAsync(string email);
    }
}