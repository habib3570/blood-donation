using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class DonorAvailabilityJob
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DonorAvailabilityJob> _logger;

        public DonorAvailabilityJob(
            ApplicationDbContext context,
            INotificationService notificationService,
            ILogger<DonorAvailabilityJob> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Donor Availability Job started");

            var now = DateTime.UtcNow;

            var donorsToActivate = await _context.DonorProfiles
                .Include(x => x.User)
                .Where(x => x.NextEligibleDate.HasValue
                         && x.NextEligibleDate.Value <= now
                         && !x.IsAvailable
                         && !x.IsVacationMode
                         && !x.User.IsBlocked)
                .ToListAsync();

            foreach (var donor in donorsToActivate)
            {
                donor.IsAvailable = true;
                donor.UpdatedAt = now;

                await _notificationService.SendNotificationAsync(
                    donor.UserId,
                    "আপনি এখন রক্ত দিতে পারবেন! 🩸",
                    "৯০ দিন পূর্ণ হয়েছে। আপনি আবার রক্ত দেওয়ার জন্য eligible হয়েছেন। আপনার একটি donation কারো জীবন বাঁচাতে পারে!",
                    Domain.Enums.NotificationType.DonationReminder);
            }

            if (donorsToActivate.Any())
                await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Donor Availability Job completed. {Count} donors activated.",
                donorsToActivate.Count);
        }
    }
}