using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class BloodStockPredictionConfiguration : IEntityTypeConfiguration<BloodStockPrediction>
    {
        public void Configure(EntityTypeBuilder<BloodStockPrediction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.BloodBank)
                .WithMany()
                .HasForeignKey(x => x.BloodBankId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.BloodBankId, x.BloodGroup });
        }
    }
}