using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class FAQConfiguration : IEntityTypeConfiguration<FAQ>
    {
        public void Configure(EntityTypeBuilder<FAQ> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Question).IsRequired().HasMaxLength(300);
            builder.Property(x => x.Answer).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.QuestionBn).HasMaxLength(300);
            builder.Property(x => x.AnswerBn).HasMaxLength(1000);

            builder.HasIndex(x => x.DisplayOrder);
        }
    }
}