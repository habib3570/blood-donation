using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.DTOs.BloodBank
{
    public class BloodStockDto
    {
        public int BloodBankId { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public string BloodGroupDisplay { get; set; } = string.Empty;
        public int UnitsAvailable { get; set; }
        public bool IsLow { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}