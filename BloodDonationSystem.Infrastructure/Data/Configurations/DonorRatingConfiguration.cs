using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class DonorRatingConfiguration : IEntityTypeConfiguration<DonorRating>
    {
        public void Configure(EntityTypeBuilder<DonorRating> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.DonorProfile)
                .WithMany()
                .HasForeignKey(r => r.DonorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Rater)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.RaterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}