using BloodDonationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationSystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DonorProfile> DonorProfiles { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<EmergencyRequest> EmergencyRequests { get; set; }
        public DbSet<EmergencyRequestAcceptance> EmergencyRequestAcceptances { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<DonorRating> DonorRatings { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<BloodBank> BloodBanks { get; set; }
        public DbSet<BloodBankStock> BloodBankStocks { get; set; }
        public DbSet<BloodStockPrediction> BloodStockPredictions { get; set; }
        public DbSet<BloodDemandHeatmap> BloodDemandHeatmaps { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationMute> NotificationMutes { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<FavoriteDonor> FavoriteDonors { get; set; }
        public DbSet<DonationCertificate> DonationCertificates { get; set; }
        public DbSet<SuccessStory> SuccessStories { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<HealthTip> HealthTips { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<RequestReport> RequestReports { get; set; }
        public DbSet<ReferralCode> ReferralCodes { get; set; }
        public DbSet<UserPoints> UserPoints { get; set; }
        public DbSet<PointTransaction> PointTransactions { get; set; }
        public DbSet<LoginActivity> LoginActivities { get; set; }
        public DbSet<PasswordChangeHistory> PasswordChangeHistories { get; set; }
        public DbSet<RecentSearch> RecentSearches { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<DonationReminder> DonationReminders { get; set; }
        public DbSet<LiveLocationShare> LiveLocationShares { get; set; }
        public DbSet<BloodRequestTemplate> BloodRequestTemplates { get; set; }
        public DbSet<DonorSpamTracker> DonorSpamTrackers { get; set; }
        public DbSet<MonthlyTopDonor> MonthlyTopDonors { get; set; }
        public DbSet<UserDonationStreak> UserDonationStreaks { get; set; }
        public DbSet<DonorEligibilityCheck> DonorEligibilityChecks { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Upazila> Upazilas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<Upazila>()
    .HasOne(u => u.District)
    .WithMany(d => d.Upazilas)
    .HasForeignKey(u => u.DistrictId)
    .OnDelete(DeleteBehavior.Restrict);
            // ── FavoriteDonor: cascade cycle ঠিক করা ──
            builder.Entity<FavoriteDonor>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoriteDonors)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FavoriteDonor>()
                .HasOne(f => f.DonorProfile)
                .WithMany()
                .HasForeignKey(f => f.DonorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Global Fix: সব Cascade Delete কে Restrict করে দেওয়া ──
            // এটা SQL Server এর "multiple cascade paths" এরর প্রতিরোধ করে
            foreach (var relationship in builder.Model.GetEntityTypes()
    .SelectMany(e => e.GetForeignKeys()))
            {
                if (relationship.DeleteBehavior == DeleteBehavior.Cascade)
                {
                    relationship.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Domain.Common.AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}