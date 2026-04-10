using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProgressConfiguration : IEntityTypeConfiguration<Progress>
{
    public void Configure(EntityTypeBuilder<Progress> builder)
    {
        builder.HasKey(x => x.Id);

        // 🔹 User relationship
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Course relationship
        builder.HasOne(x => x.Course)
               .WithMany()
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        // 🔥 One progress per user per course (CRITICAL)
        builder.HasIndex(x => new { x.UserId, x.CourseId })
               .IsUnique();

        // 🔹 TotalLessons
        builder.Property(x => x.TotalLessons)
               .IsRequired();

        // 🔹 CompletedLessons
        builder.Property(x => x.CompletedLessons)
               .IsRequired();

        // 🔹 Percentage
        builder.Property(x => x.Percentage)
               .IsRequired();

        // 🔹 Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}