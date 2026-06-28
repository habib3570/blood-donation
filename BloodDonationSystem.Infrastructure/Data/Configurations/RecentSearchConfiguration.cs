using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class RecentSearchConfiguration : IEntityTypeConfiguration<RecentSearch>
    {
        public void Configure(EntityTypeBuilder<RecentSearch> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SearchTerm).HasMaxLength(200);
            builder.Property(x => x.BloodGroup).HasMaxLength(10);
            builder.Property(x => x.District).HasMaxLength(100);
            builder.Property(x => x.Upazila).HasMaxLength(100);

            builder.HasOne(x => x.User)
                .WithMany(x => x.RecentSearches)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.SearchedAt });
        }
    }
}