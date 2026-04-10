namespace LMS.Application.DTOs.Progress;

public record CompleteLessonRequest(Guid UserId, Guid LessonId);
public record ProgressResponse(Guid UserId, Guid CourseId, int TotalLessons, int CompletedLessons, double Percentage);
public record CourseProgressCardResponse(Guid CourseId, string CourseTitle, double Percentage);
public record SkillBreakdownItemResponse(string Skill, double Score);
public record GradedWorkItemResponse(Guid SubmissionId, string Title, string CourseTitle, decimal Score, DateTime ActivityDate);
public record ProgressOverviewResponse(
    IReadOnlyCollection<CourseProgressCardResponse> CourseProgressCards,
    double OverallProgressPercentage,
    IReadOnlyCollection<SkillBreakdownItemResponse> SkillBreakdown,
    IReadOnlyCollection<GradedWorkItemResponse> GradedWork
);
