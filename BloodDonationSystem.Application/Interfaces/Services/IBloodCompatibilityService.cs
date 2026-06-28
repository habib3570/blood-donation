using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Interfaces.Services
{
    public interface IBloodCompatibilityService
    {
        List<string> GetCompatibleRecipients(BloodGroup bloodGroup);
        List<string> GetCompatibleDonors(BloodGroup bloodGroup);
        bool CanDonate(BloodGroup donorGroup, BloodGroup recipientGroup);
        string GetBloodGroupDisplayName(BloodGroup bloodGroup);
    }
}