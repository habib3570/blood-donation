using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class BloodRequestRepository : GenericRepository<BloodRequest>, IBloodRequestRepository
    {
        public BloodRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<BloodRequest>> GetByRequesterIdAsync(string requesterId)
            => await _dbSet
                .Include(x => x.Requester)
                .Include(x => x.Donor)
                .Where(x => x.RequesterId == requesterId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<BloodRequest>> GetByStatusAsync(RequestStatus status)
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<BloodRequest>> GetByBloodGroupAsync(BloodGroup bloodGroup)
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.BloodGroup == bloodGroup && x.Status == RequestStatus.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<BloodRequest>> GetByDistrictAsync(string district)
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.District == district && x.Status == RequestStatus.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<BloodRequest>> GetActiveRequestsAsync()
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.Status == RequestStatus.Pending || x.Status == RequestStatus.Accepted)
                .OrderByDescending(x => x.IsEmergency)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<BloodRequest>> GetExpiredRequestsAsync()
            => await _dbSet
                .Where(x => x.Status == RequestStatus.Pending && x.ExpiryDate.HasValue && x.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

        public async Task<BloodRequest?> GetWithDetailsAsync(int id)
            => await _dbSet
                .Include(x => x.Requester)
                .Include(x => x.Donor)
                .Include(x => x.Reports)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<int> GetTodayRequestCountByUserAsync(string userId)
            => await _dbSet.CountAsync(x => x.RequesterId == userId && x.CreatedAt.Date == DateTime.UtcNow.Date && x.IsEmergency);

        public async Task<List<BloodRequest>> GetRequestsByDonorAsync(string donorId)
            => await _dbSet
                .Include(x => x.Requester)
                .Where(x => x.DonorId == donorId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task UpdateStatusAsync(int requestId, RequestStatus status)
        {
            var request = await _dbSet.FindAsync(requestId);
            if (request != null)
            {
                request.Status = status;
                if (status == RequestStatus.Accepted) request.AcceptedAt = DateTime.UtcNow;
                if (status == RequestStatus.Completed) request.CompletedAt = DateTime.UtcNow;
                _dbSet.Update(request);
            }
        }
    }
}