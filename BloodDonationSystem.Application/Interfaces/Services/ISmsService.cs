namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendOtpAsync(string phoneNumber, string otp);
        Task SendEmergencyAlertSmsAsync(List<string> phoneNumbers, string message);
        Task SendDonationReminderSmsAsync(string phoneNumber, string donorName);
    }
}