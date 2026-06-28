using BloodDonationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonationSystem.Infrastructure.Data.Configurations
{
    public class BloodRequestTemplateConfiguration : IEntityTypeConfiguration<BloodRequestTemplate>
    {
        public void Configure(EntityTypeBuilder<BloodRequestTemplate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TemplateName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PatientName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.HospitalName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.District).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Upazila).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ContactNumber).HasMaxLength(20);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}