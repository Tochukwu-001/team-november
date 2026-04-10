using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.EntityConfigurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CertificateNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.FinalScore)
               .IsRequired()
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.IssuedAt)
               .IsRequired();

        //  User relationship
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        //  Course relationship
        builder.HasOne(x => x.Course)
               .WithMany()
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

        // UNIQUE constraint (VERY IMPORTANT)
        builder.HasIndex(x => new { x.UserId, x.CourseId })
               .IsUnique();

        //  Certificate number uniqueness
        builder.HasIndex(x => x.CertificateNumber)
               .IsUnique();

        //  Performance indexes
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CourseId);

        //  Audit fields
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
    }
}
