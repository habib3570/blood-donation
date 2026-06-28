using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.Chat
{
    public class SendMessageDto
    {
        [Required] public string ReceiverId { get; set; } = string.Empty;
        [Required, MaxLength(1000)] public string Message { get; set; } = string.Empty;
        public int? BloodRequestId { get; set; }
    }
}