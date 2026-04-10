namespace LMS.Application.DTOs.Course;

public record CreateCourseRequest(
    string Title,
    string Description,
    string Category,
    int EstimatedHours,
    string? ThumbnailUrl,
    Guid InstructorId
);

public record UpdateCourseRequest(
    string Title,
    string Description,
    string Category,
    int EstimatedHours,
    string? ThumbnailUrl
);

public record CourseResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    int EstimatedHours,
    string? ThumbnailUrl,
    Guid InstructorId,
    string InstructorName,
    int LessonCount
);

public record CourseCatalogItemResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    int EstimatedHours,
    string? ThumbnailUrl,
    string InstructorName,
    int LessonCount,
    bool IsEnrolled,
    bool IsCompleted,
    double ProgressPercentage
);

public record CourseModuleResponse(
    Guid LessonId,
    string Title,
    string? Description,
    int Order,
    int? EstimatedMinutes,
    bool IsCompleted,
    bool IsLocked,
    bool IsCurrent,
    int ContentCount
);

public record CourseInstructorResponse(
    Guid InstructorId,
    string FullName,
    string? ProfileImageUrl,
    string Headline,
    string? Bio
);

public record CourseIncludesResponse(
    int VideoLessons,
    int DownloadableResources,
    int Assignments,
    bool DiscussionForumAccess,
    bool CertificateOfCompletion
);

public record CourseDetailResponse(
    Guid Id,
    string Title,
    string Description,
    string Category,
    int EstimatedHours,
    string? ThumbnailUrl,
    CourseInstructorResponse Instructor,
    double ProgressPercentage,
    bool IsEnrolled,
    int LessonCount,
    CourseIncludesResponse Includes,
    IReadOnlyCollection<CourseModuleResponse> Modules,
    Guid? ResumeLessonId
);
