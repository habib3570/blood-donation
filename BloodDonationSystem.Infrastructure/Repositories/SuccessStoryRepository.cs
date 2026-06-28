using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Repositories
{
    public class SuccessStoryRepository : GenericRepository<SuccessStory>, ISuccessStoryRepository
    {
        public SuccessStoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<SuccessStory>> GetApprovedStoriesAsync(int page = 1, int pageSize = 10)
            => await _dbSet
                .Include(x => x.User)
                .Where(x => x.IsApproved && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        public async Task<List<SuccessStory>> GetPendingStoriesAsync()
            => await _dbSet
                .Include(x => x.User)
                .Where(x => !x.IsApproved && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task<List<SuccessStory>> GetByUserIdAsync(string userId)
            => await _dbSet
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

        public async Task ApproveStoryAsync(int storyId)
        {
            var story = await _dbSet.FindAsync(storyId);
            if (story != null)
            {
                story.IsApproved = true;
                _dbSet.Update(story);
            }
        }

        public async Task IncrementViewCountAsync(int storyId)
        {
            var story = await _dbSet.FindAsync(storyId);
            if (story != null)
            {
                story.ViewCount++;
                _dbSet.Update(story);
            }
        }
    }
}