
using LMS.Domain.Enums;

namespace LMS.Application.DTOs.Lesson;

public record CreateLessonRequest(Guid CourseId, string Title, int Order, string? Description, int? EstimatedMinutes);
public record AddLessonContentRequest(Guid LessonId, ContentType ContentType, string? Title, string? ContentUrl, string? TextContent);
public record LessonResponse(Guid Id, Guid CourseId, string Title, string? Description, int? EstimatedMinutes, int Order, int ContentCount);
public record SaveLessonNoteRequest(string Content);
public record LessonPlayerContentResponse(Guid Id, ContentType ContentType, string? Title, string? Url, string? TextContent);
public record LessonPlayerSidebarItemResponse(Guid LessonId, string Title, int Order, int? EstimatedMinutes, bool IsCompleted, bool IsLocked, bool IsCurrent);
public record LessonNoteResponse(Guid Id, string Content, DateTime UpdatedAt);
public record LessonPlayerResponse(
    Guid CourseId,
    string CourseTitle,
    Guid LessonId,
    string LessonTitle,
    string? LessonDescription,
    int LessonOrder,
    double ProgressPercentage,
    bool IsCompleted,
    Guid? PreviousLessonId,
    Guid? NextLessonId,
    IReadOnlyCollection<LessonPlayerContentResponse> Contents,
    IReadOnlyCollection<LessonPlayerSidebarItemResponse> SidebarLessons,
    LessonNoteResponse? Note
);
