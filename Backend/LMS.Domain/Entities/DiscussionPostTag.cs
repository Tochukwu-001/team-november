namespace LMS.Domain.Entities;

public class DiscussionPostTag
{
    public Guid PostId { get; private set; }
    public Guid TagId { get; private set; }

    public DiscussionPost Post { get; private set; } = default!;
    public DiscussionTag Tag { get; private set; } = default!;

    private DiscussionPostTag() { }

    public static DiscussionPostTag Create(Guid postId, Guid tagId)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post is required");

        if (tagId == Guid.Empty)
            throw new ArgumentException("Tag is required");

        return new DiscussionPostTag
        {
            PostId = postId,
            TagId = tagId
        };
    }
}
