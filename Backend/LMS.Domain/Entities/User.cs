using LMS.Domain.Enums;
namespace LMS.Domain.Entities;

public class User : BaseEntity
{
    public string PublicId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Discipline { get; private set; } = default!;
    public string? PhoneNumber { get; private set; }
    public string? CohortLabel { get; private set; }
    public string? Location { get; private set; }
    public string? ProfileImageUrl { get; private set; }
    public Guid? TeamId { get; private set; }
    public Team? Team { get; private set; }
    public string PasswordHash { get; private set; } = default!;

    public UserRole Role { get; private set; } = UserRole.Learner;

    public bool IsAdmin() => Role == UserRole.Admin;
    public bool IsInstructor() => Role == UserRole.Instructor;
    public bool IsLearner() => Role == UserRole.Learner;

    private readonly List<Enrollment> _enrollments = new();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(
        string firstName,
        string lastName,
        string email,
        string discipline,
        string passwordHash,
        UserRole role,
        Guid? teamId = null,
        string? cohortLabel = null,
        string? location = null
    )
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        if (!email.Contains("@") || !email.Contains("."))
            throw new ArgumentException("Invalid email format");

        if (string.IsNullOrWhiteSpace(discipline))
            throw new ArgumentException("Discipline is required");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required");

        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid role");

        var publicId = GeneratePublicId();
        return new User
        {
            PublicId = publicId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Discipline = discipline.Trim(),
            CohortLabel = string.IsNullOrWhiteSpace(cohortLabel) ? null : cohortLabel.Trim(),
            Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim(),
            TeamId = teamId,
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        SetUpdated();
    }

    public void UpdateProfileImage(string? profileImageUrl)
    {
        ProfileImageUrl = string.IsNullOrWhiteSpace(profileImageUrl) ? null : profileImageUrl.Trim();
        SetUpdated();
    }

    public void ChangeRole(UserRole role)
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid role");

        Role = role;
        SetUpdated();
    }

    public void SetPassword(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Password hash is required");

        PasswordHash = hashedPassword;
        SetUpdated();
    }

    public void ChangeDiscipline(string discipline)
    {
        if (string.IsNullOrWhiteSpace(discipline))
            throw new ArgumentException("Discipline is required");

        Discipline = discipline.Trim();
        SetUpdated();
    }

    public void AssignToTeam(Guid teamId)
    {
        if (teamId == Guid.Empty)
            throw new ArgumentException("Team is required");

        TeamId = teamId;
        SetUpdated();
    }

    public void RemoveFromTeam()
    {
        TeamId = null;
        SetUpdated();
    }

    private static string GeneratePublicId()
    {
        return $"TF-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }


}
