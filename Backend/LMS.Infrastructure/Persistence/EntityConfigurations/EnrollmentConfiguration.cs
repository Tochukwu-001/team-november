using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(x => x.Id);

        // User relationship
        builder.HasOne(x => x.User)
               .WithMany(x => x.Enrollments)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Course relationship
        builder.HasOne(x => x.Course)
               .WithMany(x => x.Enrollments)
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        //  Prevent duplicate enrollment (VERY IMPORTANT)
        builder.HasIndex(x => new { x.UserId, x.CourseId })
               .IsUnique();

        // EnrolledAt
        builder.Property(x => x.EnrolledAt)
               .IsRequired();

        // IsCompleted
        builder.Property(x => x.IsCompleted)
               .IsRequired();

        // CompletedAt (nullable)
        builder.Property(x => x.CompletedAt);

        // Audit fields
        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // Ignore computed property
        builder.Ignore(x => x.IsActive);
    }
}