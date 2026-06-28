using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class DonationReminderJob
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DonationReminderJob> _logger;

        public DonationReminderJob(ApplicationDbContext context, IEmailService emailService,
            INotificationService notificationService, ILogger<DonationReminderJob> logger)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Donation Reminder Job started");

            var eligibleSoon = await _context.DonorProfiles
                .Include(x => x.User)
                .Where(x => x.LastDonationDate.HasValue && x.IsAvailable && !x.User.IsBlocked)
                .ToListAsync();

            foreach (var donor in eligibleSoon)
            {
                var daysSince = (DateTime.UtcNow - donor.LastDonationDate!.Value).TotalDays;
                var daysLeft = DonationConstants.MinDaysBetweenDonations - (int)daysSince;

                if (daysLeft == 7 || daysLeft == 3 || daysLeft == 1)
                {
                    await _notificationService.SendNotificationAsync(
                        donor.UserId,
                        "Donation Reminder 🩸",
                        $"You can donate blood again in {daysLeft} day(s)!",
                        Domain.Enums.NotificationType.DonationReminder);

                    if (donor.User.Email != null && daysLeft == 7)
                        await _emailService.SendDonationReminderEmailAsync(donor.User.Email, donor.User.FullName, daysLeft);
                }
            }

            _logger.LogInformation("Donation Reminder Job completed");
        }
    }
}