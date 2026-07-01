using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class SuccessStoryCleanupJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SuccessStoryCleanupJob> _logger;

        public SuccessStoryCleanupJob(
            ApplicationDbContext context,
            ILogger<SuccessStoryCleanupJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Success Story Cleanup Job started");

            var cutoff = DateTime.UtcNow.AddHours(-24);

   
            var oldStories = await _context.SuccessStories
                .Where(x => x.CreatedAt <= cutoff)
                .ToListAsync();

            if (oldStories.Any())
            {
                _context.SuccessStories.RemoveRange(oldStories);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation(
                "Success Story Cleanup Job completed. {Count} stories deleted.",
                oldStories.Count);
        }
    }
}