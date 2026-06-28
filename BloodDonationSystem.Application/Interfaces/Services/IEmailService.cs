namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendPasswordResetEmailAsync(string to, string resetLink);
        Task SendEmailConfirmationAsync(string to, string confirmationLink);
        Task SendDonationReminderEmailAsync(string to, string donorName, int daysUntilEligible);
        Task SendBirthdayWishEmailAsync(string to, string donorName);
        Task SendEmergencyAlertEmailAsync(List<string> emails, string patientName, string bloodGroup, string hospital);
    }
}