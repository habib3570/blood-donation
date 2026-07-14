using BloodDonationSystem.Infrastructure.Data;
using BloodDonationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class RequestExpiryJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RequestExpiryJob> _logger;

        public RequestExpiryJob(
            ApplicationDbContext context,
            ILogger<RequestExpiryJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Request Expiry Job started");
            var now = DateTime.UtcNow;

            var expiredRequests = await _context.BloodRequests
                .Where(x =>
                    (x.Status == RequestStatus.Pending || x.Status == RequestStatus.Accepted)
                    && (
                        (x.RequiredDate.HasValue && x.RequiredDate.Value < now)
                        ||
                        (x.ExpiryDate.HasValue && x.ExpiryDate.Value < now)
                    ))
                .ToListAsync();

            if (expiredRequests.Any())
            {
                _context.BloodRequests.RemoveRange(expiredRequests);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation(
                "Request Expiry Job completed. {Count} requests deleted.",
                expiredRequests.Count);
        }
    }
}