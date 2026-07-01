namespace BloodDonationSystem.Application.DTOs.Statistics
{
    public class BloodGroupStatDto
    {
        public string Group { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool Hot { get; set; }
    }
}