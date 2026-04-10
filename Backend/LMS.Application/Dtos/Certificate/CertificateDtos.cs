namespace LMS.Application.DTOs.Certificate;

public record CertificateResponse(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    string UserFullName,
    string PublicId,
    string Discipline,
    string? CohortLabel,
    decimal FinalScore,
    string CertificateNumber,
    DateTime IssuedAt
);
