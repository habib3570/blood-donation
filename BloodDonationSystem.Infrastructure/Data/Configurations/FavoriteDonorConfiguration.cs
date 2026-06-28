using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class FavoriteDonorConfiguration : IEntityTypeConfiguration<FavoriteDonor>
    {
        public void Configure(EntityTypeBuilder<FavoriteDonor> builder)
        {
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.User)
                .WithMany(u => u.FavoriteDonors)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.DonorProfile)
                .WithMany()
                .HasForeignKey(f => f.DonorProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}