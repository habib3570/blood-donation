using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class UserPointsConfiguration : IEntityTypeConfiguration<UserPoints>
    {
        public void Configure(EntityTypeBuilder<UserPoints> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.UserPoints)
                .HasForeignKey(x => x.UserPointsId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TotalPoints);
        }
    }
}