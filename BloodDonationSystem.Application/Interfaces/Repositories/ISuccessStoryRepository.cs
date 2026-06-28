using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Interfaces.Repositories
{
    public interface ISuccessStoryRepository : IGenericRepository<SuccessStory>
    {
        Task<List<SuccessStory>> GetApprovedStoriesAsync(int page = 1, int pageSize = 10);
        Task<List<SuccessStory>> GetPendingStoriesAsync();
        Task<List<SuccessStory>> GetByUserIdAsync(string userId);
        Task ApproveStoryAsync(int storyId);
        Task IncrementViewCountAsync(int storyId);
    }
}