namespace BloodDonationSystem.Application.DTOs.Statistics
{
    public class PersonalStatsDto
    {
        public int TotalDonations { get; set; }
        public int AcceptedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequests { get; set; }
        public double AverageRating { get; set; }
        public int TotalPoints { get; set; }
        public int LivesSaved { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int Rank { get; set; }
        public string Level { get; set; } = string.Empty;
    }
}