namespace LMS.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public string TokenHash { get; private set; } = default!;

    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }

    private PasswordResetToken() { }

    // ?? Factory
    public static PasswordResetToken Create(Guid userId, string tokenHash, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required");

        return new PasswordResetToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt,
            IsUsed = false
        };
    }

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token already used");

        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        SetUpdated();
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

}
