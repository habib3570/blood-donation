namespace BloodDonationSystem.Application.DTOs.Location
{
    public class LocationDto
    {
        public string UserId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsLiveSharing { get; set; }
    }
}