using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DiscussionPostTagConfiguration : IEntityTypeConfiguration<DiscussionPostTag>
{
    public void Configure(EntityTypeBuilder<DiscussionPostTag> builder)
    {
        builder.HasKey(x => new { x.PostId, x.TagId });

        // Keep join rows aligned with the soft-delete filters on posts and tags.
        builder.HasQueryFilter(x => !x.Post.IsDeleted && !x.Tag.IsDeleted);

        builder.HasOne(x => x.Post)
            .WithMany(x => x.PostTags)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany(x => x.PostTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
