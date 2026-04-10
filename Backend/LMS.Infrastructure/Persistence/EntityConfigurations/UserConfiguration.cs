using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.LastName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(x => x.Email)
               .IsUnique();

        builder.Property(x => x.PublicId)
               .IsRequired()
               .HasMaxLength(32);

        builder.HasIndex(x => x.PublicId)
               .IsUnique();

        builder.Property(x => x.Discipline)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(x => x.CohortLabel)
               .HasMaxLength(50);

        builder.Property(x => x.Location)
               .HasMaxLength(150);

        builder.Property(x => x.ProfileImageUrl)
               .HasMaxLength(500);

        builder.HasIndex(x => x.TeamId);

        builder.Property(x => x.PasswordHash)
               .IsRequired();

        // Enum as string
        builder.Property(x => x.Role)
               .HasConversion<string>()
               .IsRequired();

        // Audit
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        //  Soft delete index
        builder.HasIndex(x => x.IsDeleted);

        // Relationship
        builder.HasMany(x => x.Enrollments)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Team)
               .WithMany(x => x.Members)
               .HasForeignKey(x => x.TeamId)
               .OnDelete(DeleteBehavior.SetNull);

        // Backing field
        builder.Navigation(x => x.Enrollments)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(User.Enrollments))!
               .SetField("_enrollments");

        // Ignore computed + helper methods
        builder.Ignore(x => x.FullName);
      
    }
}
