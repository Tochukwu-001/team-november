using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DiscussionReplyConfiguration : IEntityTypeConfiguration<DiscussionReply>
{
    public void Configure(EntityTypeBuilder<DiscussionReply> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.HasOne(x => x.Post)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
