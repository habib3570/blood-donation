using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class DonationConfiguration : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RecipientName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.HospitalName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.HasOne(x => x.Certificate)
                .WithOne(x => x.Donation)
                .HasForeignKey<DonationCertificate>(x => x.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.DonorProfileId);
            builder.HasIndex(x => x.DonationDate);
        }
    }
}