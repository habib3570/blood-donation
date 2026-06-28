using BloodDonationSystem.Domain.Enums;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class RequestExpiryJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RequestExpiryJob> _logger;

        public RequestExpiryJob(ApplicationDbContext context, ILogger<RequestExpiryJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Request Expiry Job started");

            var expiredBloodRequests = await _context.BloodRequests
                .Where(x => x.Status == RequestStatus.Pending
                    && x.ExpiryDate.HasValue
                    && x.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var request in expiredBloodRequests)
                request.Status = RequestStatus.Expired;

            var expiredEmergency = await _context.EmergencyRequests
                .Where(x => x.IsActive && x.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var request in expiredEmergency)
            {
                request.IsActive = false;
                request.Status = RequestStatus.Expired;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Expired {expiredBloodRequests.Count} blood requests and {expiredEmergency.Count} emergency requests");
        }
    }
}