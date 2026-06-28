using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.BloodBank;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IBloodBankService
    {
        Task<Result<List<BloodBankDto>>> GetAllBloodBanksAsync();
        Task<Result<BloodBankDto>> GetBloodBankByIdAsync(int id);
        Task<Result<List<BloodStockDto>>> GetStocksByBloodGroupAsync(BloodGroup bloodGroup);
        Task<Result<List<BloodStockDto>>> GetLowStocksAsync();
        Task<Result> UpdateStockAsync(int bloodBankId, BloodGroup bloodGroup, int units);
        Task<Result> CheckAndWarnLowStocksAsync();
        Task<Result<List<BloodStockDto>>> GetPredictionsAsync(int bloodBankId);
    }
}