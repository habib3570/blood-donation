using BloodDonationSystem.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BloodDonationSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpHost = configuration["Email:SmtpHost"] ?? configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
            _smtpUser = configuration["Email:SmtpUser"] ?? configuration["Email:Username"] ?? "";
            _smtpPass = configuration["Email:SmtpPass"] ?? configuration["Email:Password"] ?? "";
            _fromEmail = configuration["Email:FromEmail"] ?? configuration["Email:SenderEmail"] ?? "";
            _fromName = configuration["Email:FromName"] ?? configuration["Email:SenderName"] ?? "Blood Donation System";
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // ডেভেলপমেন্ট পর্যায়ে SMTP credentials ভুল/মিসিং থাকলেও
                // মূল অপারেশন (Register, Reset Password ইত্যাদি) আটকে যাবে না।
                // শুধু লগে warning থেকে যাবে।
                _logger.LogWarning(ex, "Failed to send email to {To} with subject {Subject}", to, subject);
            }
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                    <div style='background:#C41E3A;padding:20px;text-align:center;'>
                        <h1 style='color:white;'>🩸 Blood Donation System</h1>
                    </div>
                    <div style='padding:30px;background:#fff;'>
                        <h2 style='color:#1B2B5B;'>Password Reset Request</h2>
                        <p>Click the button below to reset your password:</p>
                        <a href='{resetLink}' style='background:#C41E3A;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;margin:20px 0;'>Reset Password</a>
                        <p style='color:#666;font-size:12px;'>This link expires in 24 hours.</p>
                    </div>
                </div>";

            await SendEmailAsync(to, "Password Reset - Blood Donation System", body);
        }

        public async Task SendEmailConfirmationAsync(string to, string confirmationLink)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                    <div style='background:#C41E3A;padding:20px;text-align:center;'>
                        <h1 style='color:white;'>🩸 Blood Donation System</h1>
                    </div>
                    <div style='padding:30px;background:#fff;'>
                        <h2 style='color:#1B2B5B;'>Confirm Your Email</h2>
                        <p>Thank you for registering! Please confirm your email:</p>
                        <a href='{confirmationLink}' style='background:#C41E3A;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block;margin:20px 0;'>Confirm Email</a>
                    </div>
                </div>";

            await SendEmailAsync(to, "Confirm Email - Blood Donation System", body);
        }

        public async Task SendDonationReminderEmailAsync(string to, string donorName, int daysUntilEligible)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                    <div style='background:#C41E3A;padding:20px;text-align:center;'>
                        <h1 style='color:white;'>🩸 Donation Reminder</h1>
                    </div>
                    <div style='padding:30px;background:#fff;'>
                        <h2 style='color:#1B2B5B;'>Hello, {donorName}!</h2>
                        <p>You can donate blood again in <strong>{daysUntilEligible} days</strong>.</p>
                        <p>Your donation saves lives. Thank you for being a hero! ❤️</p>
                    </div>
                </div>";

            await SendEmailAsync(to, "Donation Reminder - Blood Donation System", body);
        }

        public async Task SendBirthdayWishEmailAsync(string to, string donorName)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                    <div style='background:#C41E3A;padding:20px;text-align:center;'>
                        <h1 style='color:white;'>🎂 Happy Birthday!</h1>
                    </div>
                    <div style='padding:30px;background:#fff;text-align:center;'>
                        <h2 style='color:#1B2B5B;'>Happy Birthday, {donorName}! 🎉</h2>
                        <p style='font-size:18px;'>Thank you for being a life saver ❤️</p>
                        <p>Your kindness and generosity make the world a better place.</p>
                    </div>
                </div>";

            await SendEmailAsync(to, "Happy Birthday! - Blood Donation System", body);
        }

        public async Task SendEmergencyAlertEmailAsync(List<string> emails, string patientName, string bloodGroup, string hospital)
        {
            var body = $@"
                <div style='font-family:Arial;max-width:600px;margin:0 auto;'>
                    <div style='background:#C41E3A;padding:20px;text-align:center;'>
                        <h1 style='color:white;'>🚨 EMERGENCY BLOOD NEEDED!</h1>
                    </div>
                    <div style='padding:30px;background:#fff;'>
                        <h2 style='color:#C41E3A;'>Urgent Blood Required</h2>
                        <table style='width:100%;border-collapse:collapse;'>
                            <tr><td style='padding:8px;border:1px solid #eee;'><strong>Patient</strong></td><td style='padding:8px;border:1px solid #eee;'>{patientName}</td></tr>
                            <tr><td style='padding:8px;border:1px solid #eee;'><strong>Blood Group</strong></td><td style='padding:8px;border:1px solid #eee;color:#C41E3A;font-weight:bold;'>{bloodGroup}</td></tr>
                            <tr><td style='padding:8px;border:1px solid #eee;'><strong>Hospital</strong></td><td style='padding:8px;border:1px solid #eee;'>{hospital}</td></tr>
                        </table>
                        <p style='margin-top:20px;'>Please login to the app to respond immediately.</p>
                    </div>
                </div>";

            var tasks = emails.Select(email => SendEmailAsync(email, "🚨 EMERGENCY: Blood Needed - Blood Donation System", body));
            await Task.WhenAll(tasks);
        }
    }
}