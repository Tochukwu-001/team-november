using LMS.Application.Common;
using LMS.Application.DTOs.Progress;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IProgressRepository _progressRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly ICurrentUserService _currentUserService;

    public ProgressService(
        IProgressRepository progressRepository,
        ILessonProgressRepository lessonProgressRepository,
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository,
        ISubmissionRepository submissionRepository,
        ICertificateRepository certificateRepository,
        ICurrentUserService currentUserService)
    {
        _progressRepository = progressRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
        _submissionRepository = submissionRepository;
        _certificateRepository = certificateRepository;
        _currentUserService = currentUserService;
    }

    public async Task<BaseResponse<ProgressResponse>> GetProgressAsync(Guid userId, Guid courseId)
    {
        var progress = await _progressRepository.GetByUserAndCourseAsync(userId, courseId);
        
        if (progress == null)
            return BaseResponse<ProgressResponse>.Fail("Progress not found");

        var response = new ProgressResponse(
            progress.UserId,
            progress.CourseId,
            progress.TotalLessons,
            progress.CompletedLessons,
            progress.Percentage
        );

        return BaseResponse<ProgressResponse>.Ok(response);
    }

    public async Task<BaseResponse<ProgressOverviewResponse>> GetMyProgressAsync()
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<ProgressOverviewResponse>.Fail("Unauthorized");

        var userId = _currentUserService.UserId.Value;
        var progresses = await _progressRepository.GetByUserIdAsync(userId);
        var submissions = await _submissionRepository.GetByUserIdAsync(userId);
        var certificates = await _certificateRepository.GetByUserIdAsync(userId);

        var courseCards = progresses
            .Select(progress => new CourseProgressCardResponse(
                progress.CourseId,
                progress.Course?.Title ?? "Unknown Course",
                Math.Round(progress.Percentage, 1)))
            .ToList();

        var overallProgress = progresses.Count == 0
            ? 0
            : Math.Round(progresses.Average(x => x.Percentage), 1);

        var gradedSubmissions = submissions
            .Where(x => x.Score.HasValue && x.Assignment != null)
            .OrderByDescending(x => x.UpdatedAt ?? x.SubmittedAt)
            .ToList();

        var gradedWork = gradedSubmissions
            .Select(x => new GradedWorkItemResponse(
                x.Id,
                x.Assignment?.Title ?? "Untitled Assignment",
                x.Assignment?.Course?.Title ?? "Unknown Course",
                x.Score!.Value,
                x.UpdatedAt ?? x.SubmittedAt))
            .ToList();

        var skillBreakdown = BuildSkillBreakdown(progresses, gradedSubmissions, certificates);

        return BaseResponse<ProgressOverviewResponse>.Ok(
            new ProgressOverviewResponse(courseCards, overallProgress, skillBreakdown, gradedWork));
    }

    public async Task<BaseResponse<bool>> CompleteLessonAsync(CompleteLessonRequest request)
    {
        try 
        {
            // 1. Update LessonProgress
            var lessonProgress = await _lessonProgressRepository.GetByUserAndLessonAsync(request.UserId, request.LessonId);
            
            if (lessonProgress == null)
            {
                lessonProgress = LessonProgress.Create(request.UserId, request.LessonId);
                await _lessonProgressRepository.AddAsync(lessonProgress);
            }

            if (lessonProgress.IsCompleted)
            {
                return BaseResponse<bool>.Ok(true, "Lesson already completed");
            }

            lessonProgress.MarkAsCompleted();
            await _lessonProgressRepository.UpdateAsync(lessonProgress);

            var lessonItem = await _lessonRepository.GetByIdAsync(request.LessonId);
            if (lessonItem == null) return BaseResponse<bool>.Fail("Lesson not found");

            var courseId = lessonItem.CourseId;

            // 2. Count completed lessons
            var completedCount = await _lessonProgressRepository.GetCompletedLessonsCountAsync(request.UserId, courseId);

            // 3. Update Progress percentage
            var progress = await _progressRepository.GetByUserAndCourseAsync(request.UserId, courseId);
            if (progress != null)
            {
                progress.UpdateProgress(completedCount);
                await _progressRepository.UpdateAsync(progress);

                // 4. If 100%, mark Enrollment as completed
                if (progress.Percentage >= 100)
                {
                    var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, courseId);
                    if (enrollment != null && !enrollment.IsCompleted)
                    {
                        enrollment.MarkAsCompleted();
                        await _enrollmentRepository.UpdateAsync(enrollment);
                    }
                }
            }

            return BaseResponse<bool>.Ok(true, "Lesson completed successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<bool>.Fail(ex.Message);
        }
    }

    private static IReadOnlyCollection<SkillBreakdownItemResponse> BuildSkillBreakdown(
        IEnumerable<Progress> progresses,
        IEnumerable<Submission> gradedSubmissions,
        IEnumerable<Certificate> certificates)
    {
        var buckets = new Dictionary<string, List<double>>
        {
            ["Visual Design"] = [],
            ["User Research"] = [],
            ["Prototyping"] = [],
            ["Collaboration"] = [],
            ["Communication"] = []
        };

        foreach (var progress in progresses)
        {
            var courseText = $"{progress.Course?.Title} {progress.Course?.Category}".ToLowerInvariant();

            if (courseText.Contains("ui/ux") || courseText.Contains("design"))
                buckets["Visual Design"].Add(progress.Percentage);

            if (courseText.Contains("research") || courseText.Contains("product"))
                buckets["User Research"].Add(progress.Percentage);

            if (courseText.Contains("wireframe") || courseText.Contains("prototype") || courseText.Contains("front"))
                buckets["Prototyping"].Add(progress.Percentage);

            if (courseText.Contains("agile") || courseText.Contains("scrum") || courseText.Contains("team"))
                buckets["Collaboration"].Add(progress.Percentage);

            if (courseText.Contains("communication") || courseText.Contains("presentation") || courseText.Contains("marketing"))
                buckets["Communication"].Add(progress.Percentage);
        }

        foreach (var submission in gradedSubmissions)
        {
            var assignmentText = $"{submission.Assignment?.Title} {submission.Assignment?.Course?.Title}".ToLowerInvariant();
            var score = (double?)submission.Score ?? 0;

            if (assignmentText.Contains("design") || assignmentText.Contains("ui/ux") || assignmentText.Contains("color"))
                buckets["Visual Design"].Add(score);

            if (assignmentText.Contains("research") || assignmentText.Contains("journey"))
                buckets["User Research"].Add(score);

            if (assignmentText.Contains("wireframe") || assignmentText.Contains("prototype"))
                buckets["Prototyping"].Add(score);

            if (assignmentText.Contains("sprint") || assignmentText.Contains("retrospective") || assignmentText.Contains("team"))
                buckets["Collaboration"].Add(score);

            if (assignmentText.Contains("presentation") || assignmentText.Contains("communication"))
                buckets["Communication"].Add(score);
        }

        foreach (var certificate in certificates)
        {
            var courseText = certificate.Course?.Title?.ToLowerInvariant() ?? string.Empty;

            if (courseText.Contains("design"))
                buckets["Visual Design"].Add((double)certificate.FinalScore);

            if (courseText.Contains("research") || courseText.Contains("product"))
                buckets["User Research"].Add((double)certificate.FinalScore);
        }

        return buckets
            .Select(bucket => new SkillBreakdownItemResponse(
                bucket.Key,
                bucket.Value.Count == 0 ? 0 : Math.Round(bucket.Value.Average(), 1)))
            .ToList();
    }
}
