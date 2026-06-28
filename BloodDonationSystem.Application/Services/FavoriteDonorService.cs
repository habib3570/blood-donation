using AutoMapper;
using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;

namespace BloodDonationSystem.Application.Services
{
    public class FavoriteDonorService : IFavoriteDonorService
    {
        private readonly IFavoriteDonorRepository _favoriteDonorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteDonorService(
            IFavoriteDonorRepository favoriteDonorRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _favoriteDonorRepository = favoriteDonorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> AddFavoriteAsync(string userId, int donorProfileId)
        {
            var already = await _favoriteDonorRepository.IsFavoriteAsync(
                userId, donorProfileId);
            if (already)
                return Result.Failure("Already in favorites.");

            await _favoriteDonorRepository.AddAsync(new FavoriteDonor
            {
                UserId = userId,
                DonorProfileId = donorProfileId,
                AddedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Added to favorites.");
        }

        public async Task<Result> RemoveFavoriteAsync(string userId, int donorProfileId)
        {
            await _favoriteDonorRepository.RemoveFavoriteAsync(userId, donorProfileId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Removed from favorites.");
        }

        public async Task<Result<List<DonorDto>>> GetFavoriteDonorsAsync(string userId)
        {
            var favorites = await _favoriteDonorRepository.GetFavoritesAsync(userId);
            var donors = favorites
                .Where(f => f.DonorProfile != null)
                .Select(f => _mapper.Map<DonorDto>(f.DonorProfile))
                .ToList();
            return Result<List<DonorDto>>.Success(donors);
        }

        public async Task<Result<bool>> IsFavoriteAsync(string userId, int donorProfileId)
        {
            var isFav = await _favoriteDonorRepository.IsFavoriteAsync(
                userId, donorProfileId);
            return Result<bool>.Success(isFav);
        }
    }
}