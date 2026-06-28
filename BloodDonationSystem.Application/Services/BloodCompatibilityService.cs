using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Services
{
    public class BloodCompatibilityService : IBloodCompatibilityService
    {
        public List<string> GetCompatibleRecipients(BloodGroup bloodGroup)
        {
            var display = GetBloodGroupDisplayName(bloodGroup);
            return BloodCompatibilityConstants.CanDonateTo.ContainsKey(display)
                ? BloodCompatibilityConstants.CanDonateTo[display]
                : new List<string>();
        }

        public List<string> GetCompatibleDonors(BloodGroup bloodGroup)
        {
            var display = GetBloodGroupDisplayName(bloodGroup);
            return BloodCompatibilityConstants.CanReceiveFrom.ContainsKey(display)
                ? BloodCompatibilityConstants.CanReceiveFrom[display]
                : new List<string>();
        }

        public bool CanDonate(BloodGroup donorGroup, BloodGroup recipientGroup)
        {
            var donors = GetBloodGroupDisplayName(donorGroup);
            var recipients = GetCompatibleRecipients(donorGroup);
            return recipients.Contains(GetBloodGroupDisplayName(recipientGroup));
        }

        public string GetBloodGroupDisplayName(BloodGroup bloodGroup) => bloodGroup switch
        {
            BloodGroup.APositive => "A+",
            BloodGroup.ANegative => "A-",
            BloodGroup.BPositive => "B+",
            BloodGroup.BNegative => "B-",
            BloodGroup.ABPositive => "AB+",
            BloodGroup.ABNegative => "AB-",
            BloodGroup.OPositive => "O+",
            BloodGroup.ONegative => "O-",
            _ => bloodGroup.ToString()
        };
    }
}