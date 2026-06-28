using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Rating;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly INotificationService _notificationService;
        private readonly IPointRepository _pointRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RatingService(
            IRatingRepository ratingRepository,
            IDonorRepository donorRepository,
            INotificationService notificationService,
            IPointRepository pointRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _donorRepository = donorRepository;
            _notificationService = notificationService;
            _pointRepository = pointRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> RateDonorAsync(
            string raterId,
            int donorProfileId,
            CreateRatingDto dto)
        {
            var hasRated = await _ratingRepository.HasRatedAsync(raterId, donorProfileId);
            if (hasRated)
                return Result.Failure("You have already rated this donor.");

            if (dto.Stars < 1 || dto.Stars > 5)
                return Result.Failure("Rating must be between 1 and 5.");

            var rating = new DonorRating
            {
                DonorProfileId = donorProfileId,
                RaterId = raterId,
                Stars = dto.Stars,
                Comment = dto.Comment,
                BloodRequestId = dto.BloodRequestId,
                IsThankYouMessage = dto.IsThankYouMessage,
                CreatedAt = DateTime.UtcNow
            };

            await _ratingRepository.AddAsync(rating);

            var avgRating = await _ratingRepository.GetAverageRatingAsync(donorProfileId);
            var donor = await _donorRepository.GetByIdAsync(donorProfileId);
            if (donor != null)
            {
                donor.AverageRating = avgRating;
                donor.TotalRatings++;
                donor.UpdatedAt = DateTime.UtcNow;
                _donorRepository.Update(donor);

                await _notificationService.SendNotificationAsync(
                    donor.UserId,
                    dto.IsThankYouMessage
                        ? "Thank You Message! ❤️"
                        : $"New {dto.Stars}⭐ Rating Received!",
                    dto.IsThankYouMessage
                        ? $"Someone sent you a thank you message: {dto.Comment}"
                        : $"You received a {dto.Stars}-star rating. " +
                          $"New average: {avgRating:0.0}⭐",
                    NotificationType.SystemAlert);
            }

            await _pointRepository.AddPointsAsync(
                raterId, PointConstants.ReviewPoints,
                "Review Submitted", rating.Id.ToString());

            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Rating submitted successfully.");
        }

        public async Task<Result<List<RatingDto>>> GetDonorRatingsAsync(int donorProfileId)
        {
            var ratings = await _ratingRepository.GetByDonorProfileIdAsync(donorProfileId);
            return Result<List<RatingDto>>.Success(_mapper.Map<List<RatingDto>>(ratings));
        }

        public async Task<Result<double>> GetAverageRatingAsync(int donorProfileId)
        {
            var avg = await _ratingRepository.GetAverageRatingAsync(donorProfileId);
            return Result<double>.Success(avg);
        }

        public async Task<Result> SendThankYouMessageAsync(
            string senderId,
            int donorProfileId,
            string message)
        {
            var dto = new CreateRatingDto
            {
                Stars = 5,
                Comment = message,
                IsThankYouMessage = true
            };
            return await RateDonorAsync(senderId, donorProfileId, dto);
        }

        public async Task<Result<bool>> HasRatedAsync(string raterId, int donorProfileId)
        {
            var hasRated = await _ratingRepository.HasRatedAsync(raterId, donorProfileId);
            return Result<bool>.Success(hasRated);
        }
    }
}