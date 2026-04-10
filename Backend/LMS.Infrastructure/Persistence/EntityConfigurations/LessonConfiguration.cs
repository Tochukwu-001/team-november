using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(x => x.Id);

        // Title
        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.Property(x => x.EstimatedMinutes);

        // Order
        builder.Property(x => x.Order)
               .IsRequired();

        // Course relationship (VERY IMPORTANT)
       builder.HasOne(x => x.Course)
       .WithMany(x => x.Lessons)
       .HasForeignKey(x => x.CourseId)
       .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // LessonContent relationship (backing field)
        builder.HasMany(x => x.Contents)
       .WithOne(x => x.Lesson)
       .HasForeignKey(x => x.LessonId)
       .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Contents)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
       
        builder.Metadata
               .FindNavigation(nameof(Lesson.Contents))!
               .SetField("_contents");

        // Unique order per course
        builder.HasIndex(x => new { x.CourseId, x.Order })
               .IsUnique();

        // Ignore computed property
        builder.Ignore(x => x.ContentCount);
    }
}

