using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.HasKey(x => x.Id);

        // 🔹 User relationship
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Lesson relationship
        builder.HasOne(x => x.Lesson)
               .WithMany()
               .HasForeignKey(x => x.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

        // 🔥 Prevent duplicate tracking (VERY IMPORTANT)
        builder.HasIndex(x => new { x.UserId, x.LessonId })
               .IsUnique();

        // 🔹 IsCompleted
        builder.Property(x => x.IsCompleted)
               .IsRequired();

        // 🔹 CompletedAt (nullable)
        builder.Property(x => x.CompletedAt);

        // 🔹 Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}