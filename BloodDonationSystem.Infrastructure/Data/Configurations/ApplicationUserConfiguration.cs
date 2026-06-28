using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).HasMaxLength(300);
            builder.Property(x => x.ProfileImageUrl).HasMaxLength(500);
            builder.Property(x => x.ReferralCode).HasMaxLength(20);
            builder.Property(x => x.PreferredLanguage).HasMaxLength(5).HasDefaultValue("en");

            builder.HasOne(x => x.DonorProfile)
                .WithOne(x => x.User)
                .HasForeignKey<DonorProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UserPoints)
                .WithOne(x => x.User)
                .HasForeignKey<UserPoints>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UserPreference)
                .WithOne(x => x.User)
                .HasForeignKey<UserPreference>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}