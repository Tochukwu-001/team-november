using LMS.Application.DTOs.Course;
using LMS.Application.Interfaces.Repositories;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class CourseBrowseRepository : ICourseBrowseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseBrowseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseCatalogItemResponse>> GetCatalogAsync(Guid? userId, string? search, string? category, bool enrolledOnly)
    {
        var normalizedSearch = search?.Trim().ToLowerInvariant();
        var normalizedCategory = category?.Trim().ToLowerInvariant();

        var courses = await _context.Courses
            .AsNoTracking()
            .Include(course => course.Instructor)
            .Include(course => course.Lessons)
            .ToListAsync();

        var progresses = userId.HasValue
            ? await _context.Progresses
                .AsNoTracking()
                .Where(item => item.UserId == userId.Value)
                .ToListAsync()
            : [];

        var progressByCourseId = progresses.ToDictionary(item => item.CourseId);

        var query = courses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(course =>
                course.Title.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                course.Description.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(normalizedCategory))
        {
            query = query.Where(course =>
                string.Equals(course.Category, normalizedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (enrolledOnly)
        {
            query = query.Where(course => progressByCourseId.ContainsKey(course.Id));
        }

        return query
            .OrderBy(course => course.Title)
            .Select(course =>
            {
                var progress = progressByCourseId.GetValueOrDefault(course.Id);
                return new CourseCatalogItemResponse(
                    course.Id,
                    course.Title,
                    course.Description,
                    course.Category,
                    course.EstimatedHours,
                    course.ThumbnailUrl,
                    course.Instructor?.FullName ?? "Unknown",
                    course.Lessons.Count,
                    progress != null,
                    progress?.Percentage >= 100,
                    Math.Round(progress?.Percentage ?? 0, 1)
                );
            })
            .ToList();
    }

    public async Task<CourseDetailResponse?> GetCourseDetailAsync(Guid? userId, Guid courseId)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .Include(item => item.Instructor)
            .Include(item => item.Lessons)
                .ThenInclude(lesson => lesson.Contents)
            .FirstOrDefaultAsync(item => item.Id == courseId);

        if (course == null)
            return null;

        var progress = userId.HasValue
            ? await _context.Progresses.AsNoTracking().FirstOrDefaultAsync(item => item.UserId == userId.Value && item.CourseId == courseId)
            : null;

        var completedLessonIds = userId.HasValue
            ? await _context.LessonProgresses
                .AsNoTracking()
                .Where(item => item.UserId == userId.Value && item.Lesson.CourseId == courseId && item.IsCompleted)
                .Select(item => item.LessonId)
                .ToListAsync()
            : [];

        var assignmentsCount = await _context.Assignments
            .AsNoTracking()
            .CountAsync(item => item.CourseId == courseId);

        var instructorRoleRequest = await _context.InstructorRoleRequests
            .AsNoTracking()
            .Where(item => item.UserId == course.InstructorId && item.Status == LMS.Domain.Enums.RoleRequestStatus.Approved)
            .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
            .FirstOrDefaultAsync();

        var orderedLessons = course.Lessons
            .OrderBy(item => item.Order)
            .ToList();

        var firstIncompleteLesson = orderedLessons.FirstOrDefault(item => !completedLessonIds.Contains(item.Id));

        var modules = orderedLessons
            .Select(lesson =>
            {
                var isCompleted = completedLessonIds.Contains(lesson.Id);
                var hasIncompleteBefore = orderedLessons
                    .Where(item => item.Order < lesson.Order)
                    .Any(item => !completedLessonIds.Contains(item.Id));

                return new CourseModuleResponse(
                    lesson.Id,
                    lesson.Title,
                    lesson.Description,
                    lesson.Order,
                    lesson.EstimatedMinutes,
                    isCompleted,
                    hasIncompleteBefore && !isCompleted,
                    firstIncompleteLesson?.Id == lesson.Id,
                    lesson.Contents.Count
                );
            })
            .ToList();

        var includes = new CourseIncludesResponse(
            course.Lessons.Count,
            course.Lessons.SelectMany(item => item.Contents).Count(item => item.ContentType == LMS.Domain.Enums.ContentType.Pdf),
            assignmentsCount,
            true,
            true
        );

        return new CourseDetailResponse(
            course.Id,
            course.Title,
            course.Description,
            course.Category,
            course.EstimatedHours,
            course.ThumbnailUrl,
            new CourseInstructorResponse(
                course.InstructorId,
                course.Instructor?.FullName ?? "Unknown",
                course.Instructor?.ProfileImageUrl,
                string.IsNullOrWhiteSpace(course.Instructor?.Discipline) ? "Instructor" : $"{course.Instructor.Discipline} Instructor",
                instructorRoleRequest?.Bio
            ),
            Math.Round(progress?.Percentage ?? 0, 1),
            progress != null,
            course.Lessons.Count,
            includes,
            modules,
            firstIncompleteLesson?.Id
        );
    }
}
