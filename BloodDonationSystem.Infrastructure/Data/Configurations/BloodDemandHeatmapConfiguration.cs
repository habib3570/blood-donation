using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class BloodDemandHeatmapConfiguration : IEntityTypeConfiguration<BloodDemandHeatmap>
    {
        public void Configure(EntityTypeBuilder<BloodDemandHeatmap> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.DemandScore).HasPrecision(10, 4);

            builder.HasIndex(x => new { x.District, x.BloodGroup, x.Month, x.Year });
        }
    }
}