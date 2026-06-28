using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class LoginActivityConfiguration : IEntityTypeConfiguration<LoginActivity>
    {
        public void Configure(EntityTypeBuilder<LoginActivity> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Browser)
                .HasMaxLength(500);

            builder.HasOne(l => l.User)
                .WithMany(u => u.LoginActivities)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}