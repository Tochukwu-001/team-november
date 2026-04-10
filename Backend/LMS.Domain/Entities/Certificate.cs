namespace LMS.Domain.Entities;

public class Certificate : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = default!;

    public string CertificateNumber { get; private set; } = default!;

    public decimal FinalScore { get; private set; }

    public DateTime IssuedAt { get; private set; }

    private Certificate() { } // EF Core

    public static Certificate Create(Guid userId, Guid courseId, decimal finalScore)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User is required");

        if (courseId == Guid.Empty)
            throw new ArgumentException("Course is required");

        if (finalScore < 0 || finalScore > 100)
            throw new ArgumentException("Invalid score");

        return new Certificate
        {
            UserId = userId,
            CourseId = courseId,
            FinalScore = finalScore,
            CertificateNumber = GenerateCertificateNumber(),
            IssuedAt = DateTime.UtcNow
        };
    }

    private static string GenerateCertificateNumber()
    {
        return $"CERT-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }
}