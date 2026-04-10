using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => x.IsDeleted);

        builder.HasMany(x => x.Members)
               .WithOne(x => x.Team)
               .HasForeignKey(x => x.TeamId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.Navigation(x => x.Members)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
               .FindNavigation(nameof(Team.Members))!
               .SetField("_members");

        builder.Ignore(x => x.MemberCount);
    }
}
