namespace LMS.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    private readonly List<User> _members = new();
    public IReadOnlyCollection<User> Members => _members.AsReadOnly();

    public int MemberCount => _members.Count;

    private Team() { }

    public static Team Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Team description is required");

        return new Team
        {
            Name = name.Trim(),
            Description = description.Trim()
        };
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Team description is required");

        Name = name.Trim();
        Description = description.Trim();
        SetUpdated();
    }
}
