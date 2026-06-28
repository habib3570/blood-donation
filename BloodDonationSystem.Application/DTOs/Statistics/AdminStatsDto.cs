namespace BloodDonationSystem.Application.DTOs.Statistics
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalDonors { get; set; }
        public int VerifiedDonors { get; set; }
        public int BlockedUsers { get; set; }
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int TotalEmergencyRequests { get; set; }
        public int TotalDonations { get; set; }
        public int TotalHospitals { get; set; }
        public int FakeReports { get; set; }
        public List<object> WeeklyStats { get; set; } = new();
        public List<object> DistrictStats { get; set; } = new();
    }
}