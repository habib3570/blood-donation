using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class HealthTipConfiguration : IEntityTypeConfiguration<HealthTip>
    {
        public void Configure(EntityTypeBuilder<HealthTip> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.TitleBn).HasMaxLength(200);
            builder.Property(x => x.ContentBn).HasMaxLength(1000);
            builder.Property(x => x.Category).IsRequired().HasMaxLength(50);

            builder.HasIndex(x => x.Category);
        }
    }
}