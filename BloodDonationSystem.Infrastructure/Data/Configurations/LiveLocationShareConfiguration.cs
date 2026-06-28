using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class LiveLocationShareConfiguration : IEntityTypeConfiguration<LiveLocationShare>
    {
        public void Configure(EntityTypeBuilder<LiveLocationShare> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.EmergencyRequest)
                .WithMany()
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.DonorId, x.EmergencyRequestId });
        }
    }
}