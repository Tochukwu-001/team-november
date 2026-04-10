namespace LMS.Domain.Entities;

public class DiscussionTag : BaseEntity
{
    public string Name { get; private set; } = default!;

    private readonly List<DiscussionPostTag> _postTags = new();
    public IReadOnlyCollection<DiscussionPostTag> PostTags => _postTags.AsReadOnly();

    private DiscussionTag() { }

    public static DiscussionTag Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name is required");

        return new DiscussionTag
        {
            Name = name.Trim()
        };
    }
}
