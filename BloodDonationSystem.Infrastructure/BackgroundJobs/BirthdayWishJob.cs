using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class BirthdayWishJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BirthdayWishJob> _logger;

        public BirthdayWishJob(ApplicationDbContext context, IEmailService emailService,
            INotificationService notificationService, ILogger<BirthdayWishJob> logger)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Birthday Wish Job started");

            var today = DateTime.UtcNow;
            var birthdayUsers = await _context.Users
                .Where(x => x.DateOfBirth.Month == today.Month
                    && x.DateOfBirth.Day == today.Day
                    && !x.IsBlocked)
                .ToListAsync();

            foreach (var user in birthdayUsers)
            {
                await _notificationService.SendNotificationAsync(
                    user.Id,
                    "Happy Birthday! 🎂",
                    "Thank you for being a life saver ❤️ Have a wonderful birthday!",
                    Domain.Enums.NotificationType.BirthdayWish);

                if (user.Email != null)
                    await _emailService.SendBirthdayWishEmailAsync(user.Email, user.FullName);
            }

            _logger.LogInformation("Birthday Wish Job completed");
        }
    }
}