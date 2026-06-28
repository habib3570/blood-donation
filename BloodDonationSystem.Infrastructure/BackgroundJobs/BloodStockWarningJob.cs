using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class BloodStockWarningJob
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BloodStockWarningJob> _logger;

        public BloodStockWarningJob(ApplicationDbContext context,
            INotificationService notificationService, ILogger<BloodStockWarningJob> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Blood Stock Warning Job started");

            var lowStocks = await _context.BloodBankStocks
                .Include(x => x.BloodBank)
                .Where(x => x.UnitsAvailable <= x.CriticalLevel)
                .ToListAsync();

            var admins = await _context.Users
                .Where(x => !x.IsBlocked)
                .ToListAsync();

            foreach (var stock in lowStocks)
            {
                var bloodGroupDisplay = GetBloodGroupDisplay(stock.BloodGroup);
                foreach (var admin in admins)
                {
                    await _notificationService.SendNotificationAsync(
                        admin.Id,
                        "⚠️ Low Blood Stock Alert",
                        $"{stock.BloodBank.Name}: {bloodGroupDisplay} only {stock.UnitsAvailable} units left!",
                        Domain.Enums.NotificationType.SystemAlert);
                }
            }

            _logger.LogInformation("Blood Stock Warning Job completed");
        }

        private static string GetBloodGroupDisplay(Domain.Enums.BloodGroup bg) => bg switch
        {
            Domain.Enums.BloodGroup.APositive => "A+",
            Domain.Enums.BloodGroup.ANegative => "A-",
            Domain.Enums.BloodGroup.BPositive => "B+",
            Domain.Enums.BloodGroup.BNegative => "B-",
            Domain.Enums.BloodGroup.ABPositive => "AB+",
            Domain.Enums.BloodGroup.ABNegative => "AB-",
            Domain.Enums.BloodGroup.OPositive => "O+",
            Domain.Enums.BloodGroup.ONegative => "O-",
            _ => bg.ToString()
        };
    }
}