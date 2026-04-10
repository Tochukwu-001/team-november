namespace LMS.Application.DTOs.Enrollment;

public record EnrollRequest(Guid UserId, Guid CourseId);
public record EnrollmentResponse(Guid Id, Guid UserId, Guid CourseId, DateTime EnrolledAt, bool IsCompleted);
