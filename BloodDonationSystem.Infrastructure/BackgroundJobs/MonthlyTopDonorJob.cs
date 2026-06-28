using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BloodDonationSystem.Infrastructure.BackgroundJobs
{
    public class MonthlyTopDonorJob
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MonthlyTopDonorJob> _logger;

        public MonthlyTopDonorJob(ApplicationDbContext context, ILogger<MonthlyTopDonorJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Monthly Top Donor Job started");

            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var month = lastMonth.Month;
            var year = lastMonth.Year;

            var topDonors = await _context.Donations
                .Where(x => x.DonationDate.Month == month && x.DonationDate.Year == year)
                .GroupBy(x => x.DonorProfileId)
                .Select(g => new { DonorProfileId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            var existing = await _context.MonthlyTopDonors
                .Where(x => x.Month == month && x.Year == year)
                .ToListAsync();

            _context.MonthlyTopDonors.RemoveRange(existing);

            for (int i = 0; i < topDonors.Count; i++)
            {
                await _context.MonthlyTopDonors.AddAsync(new MonthlyTopDonor
                {
                    DonorProfileId = topDonors[i].DonorProfileId,
                    Month = month,
                    Year = year,
                    TotalDonationsThisMonth = topDonors[i].Count,
                    Rank = i + 1
                });
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Monthly Top Donor Job completed");
        }
    }
}