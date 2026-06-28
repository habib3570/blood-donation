namespace BloodDonationSystem.Application.DTOs.User
{
    public class UserPreferenceDto
    {
        public bool EmailNotification { get; set; } = true;
        public bool SmsNotification { get; set; } = true;
        public bool EmergencyAlert { get; set; } = true;
        public bool DonationReminder { get; set; } = true;
        public bool BirthdayWish { get; set; } = true;
        public string Language { get; set; } = "en";
        public bool ShowOnLeaderboard { get; set; } = true;
        public bool ShareLocation { get; set; } = false;
    }
}