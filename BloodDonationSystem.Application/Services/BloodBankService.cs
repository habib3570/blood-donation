using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.BloodBank;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class BloodBankService : IBloodBankService
    {
        private readonly IBloodBankRepository _bloodBankRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BloodBankService(
            IBloodBankRepository bloodBankRepository,
            INotificationService notificationService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _bloodBankRepository = bloodBankRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<BloodBankDto>>> GetAllBloodBanksAsync()
        {
            var banks = await _bloodBankRepository.GetAllAsync();
            return Result<List<BloodBankDto>>.Success(
                _mapper.Map<List<BloodBankDto>>(banks));
        }

        public async Task<Result<BloodBankDto>> GetBloodBankByIdAsync(int id)
        {
            var bank = await _bloodBankRepository.GetWithStocksAsync(id);
            if (bank == null)
                return Result<BloodBankDto>.Failure("Blood bank not found.");
            return Result<BloodBankDto>.Success(_mapper.Map<BloodBankDto>(bank));
        }

        public async Task<Result<List<BloodStockDto>>> GetStocksByBloodGroupAsync(
            BloodGroup bloodGroup)
        {
            var stocks = await _bloodBankRepository.GetStocksByBloodGroupAsync(bloodGroup);
            return Result<List<BloodStockDto>>.Success(
                _mapper.Map<List<BloodStockDto>>(stocks));
        }

        public async Task<Result<List<BloodStockDto>>> GetLowStocksAsync()
        {
            var stocks = await _bloodBankRepository.GetLowStocksAsync();
            return Result<List<BloodStockDto>>.Success(
                _mapper.Map<List<BloodStockDto>>(stocks));
        }

        public async Task<Result> UpdateStockAsync(
            int bloodBankId, BloodGroup bloodGroup, int units)
        {
            await _bloodBankRepository.UpdateStockAsync(bloodBankId, bloodGroup, units);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Stock updated successfully.");
        }

        public async Task<Result> CheckAndWarnLowStocksAsync()
        {
            var lowStocks = await _bloodBankRepository.GetLowStocksAsync();
            if (!lowStocks.Any())
                return Result.Success("All stocks normal.");

            var admins = await _userRepository.GetAllWithProfilesAsync();

            foreach (var stock in lowStocks)
            {
                var bgDisplay = GetBloodGroupDisplay(stock.BloodGroup);
                foreach (var admin in admins.Take(5))
                {
                    await _notificationService.SendNotificationAsync(
                        admin.Id,
                        "⚠️ Low Blood Stock",
                        $"{bgDisplay} stock is critically low: " +
                        $"{stock.UnitsAvailable} units remaining!",
                        Domain.Enums.NotificationType.SystemAlert);
                }
            }

            return Result.Success($"{lowStocks.Count} low stock warnings sent.");
        }

        public async Task<Result<List<BloodStockDto>>> GetPredictionsAsync(int bloodBankId)
        {
            var stocks = await _bloodBankRepository.GetWithStocksAsync(bloodBankId);
            if (stocks == null)
                return Result<List<BloodStockDto>>.Failure("Blood bank not found.");

            var predictions = _mapper.Map<List<BloodStockDto>>(stocks.Stocks);
            return Result<List<BloodStockDto>>.Success(predictions);
        }

        private static string GetBloodGroupDisplay(BloodGroup bg) => bg switch
        {
            BloodGroup.APositive => "A+",
            BloodGroup.ANegative => "A-",
            BloodGroup.BPositive => "B+",
            BloodGroup.BNegative => "B-",
            BloodGroup.ABPositive => "AB+",
            BloodGroup.ABNegative => "AB-",
            BloodGroup.OPositive => "O+",
            BloodGroup.ONegative => "O-",
            _ => bg.ToString()
        };
    }
}