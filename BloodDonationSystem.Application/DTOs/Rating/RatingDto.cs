namespace BloodDonationSystem.Application.DTOs.Rating
{
    public class RatingDto
    {
        public int Id { get; set; }
        public string RaterName { get; set; } = string.Empty;
        public string? RaterImageUrl { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
        public bool IsThankYouMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}