namespace LMS.Domain.Entities;

public class DiscussionReply : BaseEntity
{
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = default!;

    public DiscussionPost Post { get; private set; } = default!;
    public User User { get; private set; } = default!;

    private DiscussionReply() { }

    public static DiscussionReply Create(Guid postId, Guid userId, string content)
    {
        if (postId == Guid.Empty)
            throw new ArgumentException("Post is required");

        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Reply content is required");

        return new DiscussionReply
        {
            PostId = postId,
            UserId = userId,
            Content = content.Trim()
        };
    }
}
