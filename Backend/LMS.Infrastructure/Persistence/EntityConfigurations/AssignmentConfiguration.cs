using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.EntityConfigurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.DueDate)
               .IsRequired();

        builder.Property(x => x.CourseId)
               .IsRequired();

        //  Course relationship
        builder.HasOne(x => x.Course)
               .WithMany(x => x.Assignments) 
               .HasForeignKey(x => x.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

        //  Submissions
        builder.HasMany(x => x.Submissions)
               .WithOne(x => x.Assignment)
               .HasForeignKey(x => x.AssignmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Submissions)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(Assignment.Submissions))!
               .SetField("_submissions");

        //  Index (IMPORTANT)
        builder.HasIndex(x => x.CourseId);

        //  Audit fields
        builder.Property(x => x.CreatedAt)
               .HasDefaultValueSql("NOW()");

        builder.Property(x => x.UpdatedAt);
    }
}
