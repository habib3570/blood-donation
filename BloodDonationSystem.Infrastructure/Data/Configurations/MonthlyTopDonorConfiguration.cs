using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class MonthlyTopDonorConfiguration : IEntityTypeConfiguration<MonthlyTopDonor>
    {
        public void Configure(EntityTypeBuilder<MonthlyTopDonor> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.DonorProfile)
                .WithMany()
                .HasForeignKey(x => x.DonorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.Month, x.Year, x.Rank });
        }
    }
}