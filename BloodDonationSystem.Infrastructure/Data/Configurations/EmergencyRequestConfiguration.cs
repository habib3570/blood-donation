using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class EmergencyRequestAcceptanceConfiguration : IEntityTypeConfiguration<EmergencyRequestAcceptance>
    {
        public void Configure(EntityTypeBuilder<EmergencyRequestAcceptance> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.EmergencyRequest)
                .WithMany()
                .HasForeignKey(x => x.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.EmergencyRequestId, x.DonorId }).IsUnique();
        }
    }
}