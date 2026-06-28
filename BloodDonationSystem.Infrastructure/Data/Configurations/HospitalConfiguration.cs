using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class HospitalConfiguration : IEntityTypeConfiguration<Hospital>
    {
        public void Configure(EntityTypeBuilder<Hospital> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(300);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.EmergencyNumber).HasMaxLength(20);
            builder.Property(x => x.Website).HasMaxLength(200);

            builder.HasOne(x => x.BloodBank)
                .WithOne(x => x.Hospital)
                .HasForeignKey<BloodBank>(x => x.HospitalId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.EmergencyContacts)
                .WithOne(x => x.Hospital)
                .HasForeignKey(x => x.HospitalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.District);
        }
    }
}