using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ISuccessStoryService
    {
        Task<Result> SubmitStoryAsync(string userId, SubmitSuccessStoryDto dto);
        Task<Result<List<SuccessStoryDto>>> GetApprovedStoriesAsync(int page = 1);
        Task<Result> ApproveStoryAsync(int storyId);
        Task<Result> DeleteStoryAsync(int storyId);
        Task<Result<List<SuccessStoryDto>>> GetPendingStoriesAsync();
    }
}