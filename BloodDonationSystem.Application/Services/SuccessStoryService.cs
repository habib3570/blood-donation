using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Services
{
    public class SuccessStoryService : ISuccessStoryService
    {
        private readonly ISuccessStoryRepository _successStoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SuccessStoryService(
            ISuccessStoryRepository successStoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _successStoryRepository = successStoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> SubmitStoryAsync(string userId, SubmitSuccessStoryDto dto)
        {
            var story = new SuccessStory
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _successStoryRepository.AddAsync(story);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Story submitted for review.");
        }

        public async Task<Result<List<SuccessStoryDto>>> GetApprovedStoriesAsync(
            int page = 1)
        {
            var stories = await _successStoryRepository.GetApprovedStoriesAsync(page);
            return Result<List<SuccessStoryDto>>.Success(
                _mapper.Map<List<SuccessStoryDto>>(stories));
        }

        public async Task<Result> ApproveStoryAsync(int storyId)
        {
            await _successStoryRepository.ApproveStoryAsync(storyId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Story approved.");
        }

        public async Task<Result> DeleteStoryAsync(int storyId)
        {
            var story = await _successStoryRepository.GetByIdAsync(storyId);
            if (story == null)
                return Result.Failure("Story not found.");

            story.IsDeleted = true;
            _successStoryRepository.Update(story);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Story deleted.");
        }

        public async Task<Result<List<SuccessStoryDto>>> GetPendingStoriesAsync()
        {
            var stories = await _successStoryRepository.GetPendingStoriesAsync();
            return Result<List<SuccessStoryDto>>.Success(
                _mapper.Map<List<SuccessStoryDto>>(stories));
        }
    }
}