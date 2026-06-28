namespace BloodDonationSystem.Application.DTOs.Statistics
{
    public class MonthlyStatsDto
    {
        public int Year { get; set; }
        public List<MonthDataDto> MonthlyData { get; set; } = new();
    }

    public class MonthDataDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Donations { get; set; }
        public int AcceptedRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
}