namespace BloodDonationSystem.Domain.Constants
{
    public static class DonationConstants
    {
        public const int MinDaysBetweenDonations = 90;
        public const int MaxEmergencyRequestsPerDay = 3;
        public const int RequestExpiryHours = 6;
        public const int LivessavedPerDonation = 3;
        public const double NearbyRadiusKm1 = 1.0;
        public const double NearbyRadiusKm5 = 5.0;
        public const double NearbyRadiusKm10 = 10.0;
        public const int MinDonorAge = 18;
        public const int MaxDonorAge = 60;
        public const double MinWeightKg = 50.0;
    }
}