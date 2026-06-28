using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class BloodBankConfiguration : IEntityTypeConfiguration<BloodBank>
    {
        public void Configure(EntityTypeBuilder<BloodBank> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(300);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);

            builder.HasMany(x => x.Stocks)
                .WithOne(x => x.BloodBank)
                .HasForeignKey(x => x.BloodBankId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}