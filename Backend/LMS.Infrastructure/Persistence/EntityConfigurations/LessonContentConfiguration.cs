using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonContentConfiguration : IEntityTypeConfiguration<LessonContent>
{
    public void Configure(EntityTypeBuilder<LessonContent> builder)
    {
        builder.HasKey(x => x.Id);

        // 🔹 ContentType (store as string for readability)
        builder.Property(x => x.ContentType)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(x => x.Title)
               .HasMaxLength(200);

        // 🔹 URL (optional)
        builder.Property(x => x.Url)
               .HasMaxLength(1000);

        // 🔹 TextContent (optional)
        builder.Property(x => x.TextContent)
              .HasColumnType("text");

        // 🔹 Relationship with Lesson
        builder.HasOne(x => x.Lesson)
               .WithMany(x => x.Contents)
               .HasForeignKey(x => x.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

        // 🔹 Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);
    }
}
