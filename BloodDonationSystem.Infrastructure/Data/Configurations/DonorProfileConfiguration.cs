using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class DonorProfileConfiguration : IEntityTypeConfiguration<DonorProfile>
    {
        public void Configure(EntityTypeBuilder<DonorProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PreferredArea).HasMaxLength(200);
            builder.Property(x => x.PreferredDistrict).HasMaxLength(100);
            builder.Property(x => x.PreferredUpazila).HasMaxLength(100);
            builder.Property(x => x.AverageRating).HasPrecision(3, 2);
            builder.Property(x => x.SmartPriorityScore).HasPrecision(10, 4);

            builder.HasMany(x => x.Donations)
                .WithOne(x => x.DonorProfile)
                .HasForeignKey(x => x.DonorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Ratings)
                .WithOne(x => x.DonorProfile)
                .HasForeignKey(x => x.DonorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Badges)
                .WithOne(x => x.DonorProfile)
                .HasForeignKey(x => x.DonorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Achievements)
                .WithOne(x => x.DonorProfile)
                .HasForeignKey(x => x.DonorProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}