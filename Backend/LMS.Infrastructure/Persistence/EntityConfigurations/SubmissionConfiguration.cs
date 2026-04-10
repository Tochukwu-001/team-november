using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.EntityConfigurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Answer)
               .HasMaxLength(5000);

        builder.Property(x => x.AttachmentUrl)
               .HasMaxLength(1000);

        builder.Property(x => x.AttachmentName)
               .HasMaxLength(255);

        builder.Property(x => x.AttachmentContentType)
               .HasMaxLength(150);

        builder.Property(x => x.AttachmentSizeBytes);

        builder.Property(x => x.Score)
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.SubmittedAt)
               .IsRequired();

        // Assignment relationship
        builder.HasOne(x => x.Assignment)
               .WithMany(x => x.Submissions)
               .HasForeignKey(x => x.AssignmentId)
               .OnDelete(DeleteBehavior.Cascade);

        //  User relationship
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // UNIQUE constraint (VERY IMPORTANT)
        builder.HasIndex(x => new { x.AssignmentId, x.UserId })
               .IsUnique();

        //  Performance indexes
        builder.HasIndex(x => x.AssignmentId);
        builder.HasIndex(x => x.UserId);

        //  Audit fields
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
    }
}
