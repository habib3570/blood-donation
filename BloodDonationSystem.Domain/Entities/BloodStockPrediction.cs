using BloodDonationSystem.Domain.Common;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Domain.Entities
{
    public class BloodStockPrediction : BaseEntity
    {
        public int BloodBankId { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public int CurrentStock { get; set; }
        public int PredictedDemand { get; set; }
        public int EstimatedDaysRemaining { get; set; }
        public bool IsCritical { get; set; } = false;
        public bool WarningIssued { get; set; } = false;
        public DateTime PredictionDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public BloodBank BloodBank { get; set; } = null!;
    }
}