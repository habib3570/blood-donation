using BloodDonationSystem.Domain.Common;

namespace BloodDonationSystem.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        public int? BloodRequestId { get; set; }

        // Navigation Properties
        public ApplicationUser Sender { get; set; } = null!;
        public ApplicationUser Receiver { get; set; } = null!;
    }
}