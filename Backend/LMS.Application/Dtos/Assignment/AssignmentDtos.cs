namespace LMS.Application.DTOs.Assignment;
public record CreateAssignmentRequest( Guid CourseId, string Title, string Description, DateTime DueDate);

public record UpdateAssignmentRequest(string Title, string Description, DateTime DueDate);

public record AssignmentResponse(Guid Id, Guid CourseId, string CourseTitle, string Title, string Description, DateTime DueDate, bool IsPastDue);

public record LearnerAssignmentResponse(
    Guid AssignmentId,
    Guid CourseId,
    string CourseTitle,
    string Title,
    string Description,
    DateTime DueDate,
    string Status,
    bool IsPastDue,
    Guid? SubmissionId,
    string? SubmissionAnswer,
    string? SubmissionAttachmentUrl,
    string? SubmissionAttachmentName,
    string? SubmissionAttachmentContentType,
    long? SubmissionAttachmentSizeBytes,
    DateTime? SubmittedAt,
    decimal? Score,
    string? Feedback
);

public record SubmitAssignmentRequest(Guid AssignmentId, string? Answer);

public record SubmissionResponse(
    Guid Id,
    Guid AssignmentId,
    Guid UserId,
    string? Answer,
    string? AttachmentUrl,
    string? AttachmentName,
    string? AttachmentContentType,
    long? AttachmentSizeBytes,
    decimal? Score,
    string? Feedback,
    DateTime SubmittedAt,
    bool IsGraded);

public record GradeSubmissionRequest(decimal Score, string? Feedback);
