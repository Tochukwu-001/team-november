using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(x => x.Category)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.EstimatedHours)
               .IsRequired();

        builder.Property(x => x.ThumbnailUrl)
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.Instructor)
               .WithMany()
               .HasForeignKey(x => x.InstructorId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.InstructorId);

        builder.HasMany(x => x.Lessons)
               .WithOne(x => x.Course)
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Lessons)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(Course.Lessons))!
               .SetField("_lessons");

        builder.Navigation(x => x.Assignments)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(Course.Assignments))!
               .SetField("_assignments");

        builder.HasMany(x => x.Enrollments)
               .WithOne(x => x.Course)
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Enrollments)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(Course.Enrollments))!
               .SetField("_enrollments");

        builder.Ignore(x => x.LessonCount);
    }
}

