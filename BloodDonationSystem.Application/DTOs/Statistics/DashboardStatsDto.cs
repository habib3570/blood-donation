namespace BloodDonationSystem.Application.DTOs.Statistics
{
    public class DashboardStatsDto
    {
        public int TotalDonors { get; set; }
        public int TotalDonations { get; set; }
        public int ActiveDonors { get; set; }
        public int EmergencyRequests { get; set; }
        public int LivesSaved { get; set; }
        public int TotalHospitals { get; set; }
        public int TotalBloodBanks { get; set; }
        public int PendingRequests { get; set; }
        public int NewDonorsThisMonth { get; set; }
        public int NewDonationsThisWeek { get; set; }
        public List<object> MonthlyDonationChart { get; set; } = new();
        public List<BloodGroupStatDto> BloodGroupDistribution { get; set; } = new();
        public List<object> TopDistricts { get; set; } = new();
    }
}