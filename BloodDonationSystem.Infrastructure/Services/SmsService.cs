using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace BloodDonationSystem.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _senderId;
        private readonly HttpClient _httpClient;

        public SmsService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _apiKey = configuration["Sms:ApiKey"] ?? "";
            _senderId = configuration["Sms:SenderId"] ?? "BloodDonate";
            _httpClient = httpClient;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var url = $"https://api.sms.net.bd/sendsms?api_key={_apiKey}&msg={Uri.EscapeDataString(message)}&to={phoneNumber}";
                await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMS Error: {ex.Message}");
            }
        }

        public async Task SendOtpAsync(string phoneNumber, string otp)
        {
            var message = $"Your Blood Donation System OTP is: {otp}. Valid for 10 minutes.";
            await SendSmsAsync(phoneNumber, message);
        }

        public async Task SendEmergencyAlertSmsAsync(List<string> phoneNumbers, string message)
        {
            var tasks = phoneNumbers.Select(phone => SendSmsAsync(phone, message));
            await Task.WhenAll(tasks);
        }

        public async Task SendDonationReminderSmsAsync(string phoneNumber, string donorName)
        {
            var message = $"Dear {donorName}, you are now eligible to donate blood again. Your donation saves lives! - Blood Donation System";
            await SendSmsAsync(phoneNumber, message);
        }
    }
}