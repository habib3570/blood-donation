using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.Rating
{
    public class CreateRatingDto
    {
        [Range(1, 5)] public int Stars { get; set; }
        [MaxLength(500)] public string? Comment { get; set; }
        public int? BloodRequestId { get; set; }
        public bool IsThankYouMessage { get; set; } = false;
    }
}