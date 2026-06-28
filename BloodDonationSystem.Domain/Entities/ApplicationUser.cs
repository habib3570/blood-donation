using BloodDonationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BloodDonationSystem.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string? PreferredLanguage { get; set; } = "en";
        public int FontSizePreference { get; set; } = 14;
        public bool DarkMode { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeenAt { get; set; }
        public string? ReferralCode { get; set; }
        public string? ReferredByUserId { get; set; }

        // Navigation Properties
        public DonorProfile? DonorProfile { get; set; }
        public ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<DonorRating> GivenRatings { get; set; } = new List<DonorRating>();
        public ICollection<FavoriteDonor> FavoriteDonors { get; set; } = new List<FavoriteDonor>();
        public ICollection<LoginActivity> LoginActivities { get; set; } = new List<LoginActivity>();
        public ICollection<PasswordChangeHistory> PasswordChangeHistories { get; set; } = new List<PasswordChangeHistory>();
        public ICollection<RecentSearch> RecentSearches { get; set; } = new List<RecentSearch>();
        public UserPoints? UserPoints { get; set; }
        public UserPreference? UserPreference { get; set; }
    }
}