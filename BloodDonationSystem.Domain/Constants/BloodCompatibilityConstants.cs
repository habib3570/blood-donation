namespace BloodDonationSystem.Domain.Constants
{
    public static class BloodCompatibilityConstants
    {
        public static readonly Dictionary<string, List<string>> CanDonateTo = new()
        {
            { "O-",  new List<string> { "O-", "O+", "A-", "A+", "B-", "B+", "AB-", "AB+" } },
            { "O+",  new List<string> { "O+", "A+", "B+", "AB+" } },
            { "A-",  new List<string> { "A-", "A+", "AB-", "AB+" } },
            { "A+",  new List<string> { "A+", "AB+" } },
            { "B-",  new List<string> { "B-", "B+", "AB-", "AB+" } },
            { "B+",  new List<string> { "B+", "AB+" } },
            { "AB-", new List<string> { "AB-", "AB+" } },
            { "AB+", new List<string> { "AB+" } }
        };

        public static readonly Dictionary<string, List<string>> CanReceiveFrom = new()
        {
            { "O-",  new List<string> { "O-" } },
            { "O+",  new List<string> { "O-", "O+" } },
            { "A-",  new List<string> { "O-", "A-" } },
            { "A+",  new List<string> { "O-", "O+", "A-", "A+" } },
            { "B-",  new List<string> { "O-", "B-" } },
            { "B+",  new List<string> { "O-", "O+", "B-", "B+" } },
            { "AB-", new List<string> { "O-", "A-", "B-", "AB-" } },
            { "AB+", new List<string> { "O-", "O+", "A-", "A+", "B-", "B+", "AB-", "AB+" } }
        };
    }
}