using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class BloodRequestConfiguration : IEntityTypeConfiguration<BloodRequest>
    {
        public void Configure(EntityTypeBuilder<BloodRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PatientName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.HospitalName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.HospitalAddress).HasMaxLength(300);
            builder.Property(x => x.ContactNumber).HasMaxLength(20);
            builder.Property(x => x.AdditionalInfo).HasMaxLength(500);

            builder.HasOne(x => x.Requester)
                .WithMany(x => x.BloodRequests)
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Donor)
                .WithMany()
                .HasForeignKey(x => x.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Reports)
                .WithOne(x => x.BloodRequest)
                .HasForeignKey(x => x.BloodRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.BloodGroup);
            builder.HasIndex(x => x.District);
        }
    }
}