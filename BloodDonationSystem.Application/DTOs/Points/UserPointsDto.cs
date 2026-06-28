namespace BloodDonationSystem.Application.DTOs.Points
{
    public class UserPointsDto
    {
        public string UserId { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int CurrentMonthPoints { get; set; }
        public int Rank { get; set; }
        public string? UserFullName { get; set; }
        public string? BloodGroupDisplay { get; set; }
        public string? District { get; set; }
    }

    public class PointTransactionDto
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}