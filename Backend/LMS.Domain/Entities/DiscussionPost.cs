namespace LMS.Domain.Entities;

public class DiscussionPost : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;

    public User User { get; private set; } = default!;

    private readonly List<DiscussionReply> _replies = new();
    public IReadOnlyCollection<DiscussionReply> Replies => _replies.AsReadOnly();

    private readonly List<DiscussionPostTag> _postTags = new();
    public IReadOnlyCollection<DiscussionPostTag> PostTags => _postTags.AsReadOnly();

    private DiscussionPost() { }

    public static DiscussionPost Create(Guid userId, string title, string content)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required");

        return new DiscussionPost
        {
            UserId = userId,
            Title = title.Trim(),
            Content = content.Trim()
        };
    }

    public void AddReply(DiscussionReply reply)
    {
        ArgumentNullException.ThrowIfNull(reply);
        _replies.Add(reply);
    }

    public void AddTag(DiscussionPostTag postTag)
    {
        ArgumentNullException.ThrowIfNull(postTag);
        _postTags.Add(postTag);
    }
}
