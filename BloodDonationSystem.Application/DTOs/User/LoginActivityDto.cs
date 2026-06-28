namespace BloodDonationSystem.Application.DTOs.User
{
    public class LoginActivityDto
    {
        public int Id { get; set; }
        public string? IpAddress { get; set; }
        public string? DeviceInfo { get; set; }
        public string? Browser { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime LoginAt { get; set; }
    }
}