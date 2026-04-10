using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InstructorRoleRequestConfiguration : IEntityTypeConfiguration<InstructorRoleRequest>
{
    public void Configure(EntityTypeBuilder<InstructorRoleRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Bio)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.Expertise)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}
