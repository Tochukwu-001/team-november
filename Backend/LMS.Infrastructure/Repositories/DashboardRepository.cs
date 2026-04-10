using LMS.Application.DTOs.Dashboard;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Enums;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse?> GetDashboardAsync(
        Guid userId,
        int continueLearningLimit = 3,
        int deadlineLimit = 3,
        int activityLimit = 5,
        int teamMemberPreviewLimit = 3
    )
    {
        var now = DateTime.UtcNow;

        var user = await _context.Users
            .AsNoTracking()
            .Include(item => item.Team!)
                .ThenInclude(team => team.Members)
            .FirstOrDefaultAsync(item => item.Id == userId);

        if (user == null)
            return null;

        var progresses = await _context.Progresses
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Course)
                .ThenInclude(course => course.Instructor)
            .ToListAsync();

        var enrolledCourseIds = progresses
            .Select(item => item.CourseId)
            .Distinct()
            .ToList();

        var enrolledCoursesCount = enrolledCourseIds.Count;
        var averageCompletion = enrolledCoursesCount == 0
            ? 0
            : Math.Round(progresses.Average(item => item.Percentage), 1);

        var certificatesCount = await _context.Certificates
            .AsNoTracking()
            .CountAsync(item => item.UserId == userId);

        var pendingTasksCount = await _context.Assignments
            .AsNoTracking()
            .Where(item => enrolledCourseIds.Contains(item.CourseId) && item.DueDate >= now)
            .Where(item => !_context.Submissions.Any(submission =>
                submission.AssignmentId == item.Id && submission.UserId == userId))
            .CountAsync();

        var continueLearning = progresses
            .Where(item => item.Percentage < 100)
            .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
            .Take(continueLearningLimit)
            .Select(item => new ContinueLearningItemResponse(
                item.CourseId,
                item.Course.Title,
                item.Course.Instructor?.FullName ?? "Unknown",
                Math.Round(item.Percentage, 1),
                item.UpdatedAt ?? item.CreatedAt
            ))
            .ToList();

        var upcomingDeadlines = await _context.Assignments
            .AsNoTracking()
            .Where(item => enrolledCourseIds.Contains(item.CourseId) && item.DueDate >= now)
            .Where(item => !_context.Submissions.Any(submission =>
                submission.AssignmentId == item.Id && submission.UserId == userId))
            .OrderBy(item => item.DueDate)
            .Include(item => item.Course)
            .Take(deadlineLimit)
            .Select(item => new UpcomingDeadlineResponse(
                item.Id,
                item.Title,
                item.Course.Title,
                item.DueDate,
                Math.Max(0, (int)Math.Ceiling((item.DueDate - now).TotalDays))
            ))
            .ToListAsync();

        var lessonActivities = await _context.LessonProgresses
            .AsNoTracking()
            .Where(item => item.UserId == userId && item.CompletedAt != null)
            .Include(item => item.Lesson)
                .ThenInclude(lesson => lesson.Course)
            .Select(item => new RecentActivityResponse(
                "lesson_completed",
                $"You completed lesson {item.Lesson.Title} in {item.Lesson.Course.Title}",
                item.CompletedAt!.Value
            ))
            .ToListAsync();

        var submissionActivities = await _context.Submissions
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Assignment)
            .Select(item => new RecentActivityResponse(
                "assignment_submitted",
                $"You submitted {item.Assignment.Title}",
                item.SubmittedAt
            ))
            .ToListAsync();

        var certificateActivities = await _context.Certificates
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Course)
            .Select(item => new RecentActivityResponse(
                "certificate_earned",
                $"You earned a certificate in {item.Course.Title}",
                item.IssuedAt
            ))
            .ToListAsync();

        var discussionPostActivities = await _context.DiscussionPosts
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Select(item => new RecentActivityResponse(
                "discussion_posted",
                $"You started the discussion {item.Title}",
                item.CreatedAt
            ))
            .ToListAsync();

        var discussionReplyActivities = await _context.DiscussionReplies
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Post)
            .Select(item => new RecentActivityResponse(
                "discussion_replied",
                $"You replied to {item.Post.Title}",
                item.CreatedAt
            ))
            .ToListAsync();

        var enrollmentActivities = await _context.Enrollments
            .AsNoTracking()
            .Where(item => item.UserId == userId)
            .Include(item => item.Course)
            .Select(item => new RecentActivityResponse(
                "course_joined",
                $"You enrolled in {item.Course.Title}",
                item.EnrolledAt
            ))
            .ToListAsync();

        var recentActivity = lessonActivities
            .Concat(submissionActivities)
            .Concat(certificateActivities)
            .Concat(discussionPostActivities)
            .Concat(discussionReplyActivities)
            .Concat(enrollmentActivities)
            .OrderByDescending(item => item.OccurredAt)
            .Take(activityLimit)
            .ToList();

        TeamPreviewResponse? myTeam = null;
        if (user.Team != null)
        {
            var orderedMembers = user.Team.Members
                .OrderByDescending(member => member.Id == userId)
                .ThenBy(member => member.FirstName)
                .ThenBy(member => member.LastName)
                .ToList();

            myTeam = new TeamPreviewResponse(
                user.Team.Id,
                user.Team.Name,
                orderedMembers.Count,
                orderedMembers
                    .Take(teamMemberPreviewLimit)
                    .Select(member => new TeamPreviewMemberResponse(
                        member.Id,
                        member.FullName,
                        member.Discipline,
                        member.Id == userId
                    ))
                    .ToList()
            );
        }

        return new DashboardResponse(
            GetGreeting(now),
            user.FullName,
            new DashboardMetricCardsResponse(
                enrolledCoursesCount,
                averageCompletion,
                pendingTasksCount,
                certificatesCount
            ),
            continueLearning,
            upcomingDeadlines,
            recentActivity,
            myTeam,
            new InternIdentityCardResponse(
                user.FullName,
                user.PublicId,
                user.Discipline,
                user.CohortLabel,
                user.Location,
                user.Role.ToString(),
                user.Team?.Name
            )
        );
    }

    private static string GetGreeting(DateTime now)
    {
        var hour = now.Hour;

        if (hour < 12)
            return "Good morning";

        if (hour < 17)
            return "Good afternoon";

        return "Good evening";
    }

    public async Task<AdminDashboardResponse?> GetAdminDashboardAsync(Guid userId, int activityLimit = 8)
    {
        var now = DateTime.UtcNow;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId);

        if (user == null)
            return null;

        var totalUsers = await _context.Users.AsNoTracking().CountAsync();
        var totalLearners = await _context.Users.AsNoTracking().CountAsync(x => x.Role == UserRole.Learner);
        var totalInstructors = await _context.Users.AsNoTracking().CountAsync(x => x.Role == UserRole.Instructor);
        var totalAdmins = await _context.Users.AsNoTracking().CountAsync(x => x.Role == UserRole.Admin);
        var pendingInstructorRequests = await _context.InstructorRoleRequests.AsNoTracking().CountAsync(x => x.Status == RoleRequestStatus.Pending);
        var totalTeams = await _context.Teams.AsNoTracking().CountAsync();
        var totalCourses = await _context.Courses.AsNoTracking().CountAsync();
        var totalAssignments = await _context.Assignments.AsNoTracking().CountAsync();
        var totalSubmissions = await _context.Submissions.AsNoTracking().CountAsync();
        var totalCertificates = await _context.Certificates.AsNoTracking().CountAsync();
        var totalDiscussionPosts = await _context.DiscussionPosts.AsNoTracking().CountAsync();

        var userActivities = await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminRecentActivityResponse(
                "user_registered",
                $"New user registered: {x.FullName}",
                x.CreatedAt))
            .ToListAsync();

        var roleRequestActivities = await _context.InstructorRoleRequests
            .AsNoTracking()
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminRecentActivityResponse(
                "role_request",
                $"{x.User.FullName} submitted an instructor role request",
                x.CreatedAt))
            .ToListAsync();

        var courseActivities = await _context.Courses
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminRecentActivityResponse(
                "course_created",
                $"New course created: {x.Title}",
                x.CreatedAt))
            .ToListAsync();

        var discussionActivities = await _context.DiscussionPosts
            .AsNoTracking()
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .Take(activityLimit)
            .Select(x => new AdminRecentActivityResponse(
                "discussion_posted",
                $"{x.User.FullName} started a discussion: {x.Title}",
                x.CreatedAt))
            .ToListAsync();

        var recentActivity = userActivities
            .Concat(roleRequestActivities)
            .Concat(courseActivities)
            .Concat(discussionActivities)
            .OrderByDescending(x => x.OccurredAt)
            .Take(activityLimit)
            .ToList();

        return new AdminDashboardResponse(
            GetGreeting(now),
            user.FullName,
            new AdminDashboardMetricCardsResponse(
                totalUsers,
                totalLearners,
                totalInstructors,
                totalAdmins,
                pendingInstructorRequests,
                totalTeams,
                totalCourses,
                totalAssignments,
                totalSubmissions,
                totalCertificates,
                totalDiscussionPosts
            ),
            recentActivity
        );
    }

    public async Task<InstructorDashboardResponse?> GetInstructorDashboardAsync(
        Guid userId,
        int courseLimit = 6,
        int reviewLimit = 8,
        int activityLimit = 8)
    {
        var now = DateTime.UtcNow;

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId);

        if (user == null)
            return null;

        var courses = await _context.Courses
            .AsNoTracking()
            .Where(item => item.InstructorId == userId)
            .Include(item => item.Lessons)
            .Include(item => item.Assignments)
            .Include(item => item.Enrollments)
            .OrderByDescending(item => item.CreatedAt)
            .ToListAsync();

        var courseIds = courses.Select(item => item.Id).ToList();

        var submissions = await _context.Submissions
            .AsNoTracking()
            .Where(item => courseIds.Contains(item.Assignment.CourseId))
            .Include(item => item.Assignment)
                .ThenInclude(assignment => assignment.Course)
            .Include(item => item.User)
            .ToListAsync();

        var totalCourses = courses.Count;
        var totalLessons = courses.Sum(item => item.Lessons.Count);
        var totalAssignments = courses.Sum(item => item.Assignments.Count);
        var totalSubmissions = submissions.Count;
        var pendingGrading = submissions.Count(item => !item.Score.HasValue);
        var totalLearners = courses
            .SelectMany(item => item.Enrollments)
            .Select(item => item.UserId)
            .Distinct()
            .Count();

        var courseOverview = courses
            .Take(courseLimit)
            .Select(item => new InstructorCourseOverviewResponse(
                item.Id,
                item.Title,
                item.Category,
                item.Lessons.Count,
                item.Assignments.Count,
                item.Enrollments.Select(x => x.UserId).Distinct().Count()
            ))
            .ToList();

        var pendingReviews = submissions
            .Where(item => !item.Score.HasValue)
            .OrderByDescending(item => item.SubmittedAt)
            .Take(reviewLimit)
            .Select(item => new InstructorSubmissionReviewResponse(
                item.Id,
                item.AssignmentId,
                item.Assignment.Title,
                item.Assignment.Course.Title,
                item.User.FullName,
                item.SubmittedAt
            ))
            .ToList();

        var courseActivities = courses
            .Select(item => new InstructorRecentActivityResponse(
                "course_created",
                $"You created the course {item.Title}",
                item.CreatedAt
            ));

        var lessonActivities = await _context.Lessons
            .AsNoTracking()
            .Where(item => courseIds.Contains(item.CourseId))
            .Include(item => item.Course)
            .Select(item => new InstructorRecentActivityResponse(
                "lesson_created",
                $"Lesson {item.Title} was added to {item.Course.Title}",
                item.CreatedAt
            ))
            .ToListAsync();

        var assignmentActivities = await _context.Assignments
            .AsNoTracking()
            .Where(item => courseIds.Contains(item.CourseId))
            .Include(item => item.Course)
            .Select(item => new InstructorRecentActivityResponse(
                "assignment_created",
                $"Assignment {item.Title} was created for {item.Course.Title}",
                item.CreatedAt
            ))
            .ToListAsync();

        var submissionActivities = submissions
            .Select(item => new InstructorRecentActivityResponse(
                "submission_received",
                $"{item.User.FullName} submitted {item.Assignment.Title}",
                item.SubmittedAt
            ));

        var recentActivity = courseActivities
            .Concat(lessonActivities)
            .Concat(assignmentActivities)
            .Concat(submissionActivities)
            .OrderByDescending(item => item.OccurredAt)
            .Take(activityLimit)
            .ToList();

        return new InstructorDashboardResponse(
            GetGreeting(now),
            user.FullName,
            new InstructorDashboardMetricCardsResponse(
                totalCourses,
                totalLessons,
                totalAssignments,
                totalSubmissions,
                pendingGrading,
                totalLearners
            ),
            courseOverview,
            pendingReviews,
            recentActivity
        );
    }
}
