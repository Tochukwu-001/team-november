using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DiscussionTagConfiguration : IEntityTypeConfiguration<DiscussionTag>
{
    public void Configure(EntityTypeBuilder<DiscussionTag> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.PostTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.PostTags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(DiscussionTag.PostTags))!
            .SetField("_postTags");
    }
}
