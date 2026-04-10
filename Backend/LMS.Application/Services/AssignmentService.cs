using LMS.Application.Common;
using LMS.Application.DTOs.Assignment;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using static LMS.Application.Services.SubmissionService;

namespace LMS.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignmentService> _logger;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ICourseRepository courseRepository,
        INotificationService notificationService,
        ISubmissionRepository submissionRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<AssignmentService> logger)
    {
        _assignmentRepository = assignmentRepository;
        _courseRepository = courseRepository;
        _submissionRepository = submissionRepository;
        _currentUserService = currentUserService;
        _enrollmentRepository = enrollmentRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse<AssignmentResponse>> CreateAssignmentAsync(CreateAssignmentRequest request)
    {
        _logger.LogInformation("Creating assignment for course {CourseId}", request.CourseId);

        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            _logger.LogWarning("Course not found: {CourseId}", request.CourseId);
            return BaseResponse<AssignmentResponse>.Fail("Course not found");
        }

        var assignment = Assignment.Create(
            request.CourseId,
            request.Title,
            request.Description,
            request.DueDate
        );

        await _assignmentRepository.AddAsync(assignment);
        await _unitOfWork.SaveChangesAsync();

        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId);
        await _notificationService.NotifyUsersAsync(enrollments.Select(enrollment =>
            new LMS.Application.DTOs.Notification.CreateNotificationRequest(
                enrollment.UserId,
                LMS.Domain.Enums.NotificationType.AssignmentPosted,
                "New Assignment Posted",
                $"{assignment.Title} has been posted for {course.Title}.",
                "/assignments"
            )));

        return BaseResponse<AssignmentResponse>.Ok(
            MapToResponse(assignment),
            "Assignment created successfully"
        );
    }

    public async Task<BaseResponse<PagedResult<AssignmentResponse>>> GetPagedAsync(int page, int pageSize)
    {
        var result = await _assignmentRepository.GetPagedAsync(page, pageSize);

        var mapped = result.Items.Select(MapToResponse).ToList();

        return BaseResponse<PagedResult<AssignmentResponse>>.Ok(new PagedResult<AssignmentResponse>
        {
            Items = mapped,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        });
    }

    public async Task<BaseResponse<IEnumerable<AssignmentResponse>>> GetAssignmentsByCourseIdAsync(Guid courseId)
    {
        var assignments = await _assignmentRepository.GetByCourseIdAsync(courseId);

        if (!assignments.Any())
            return BaseResponse<IEnumerable<AssignmentResponse>>.Ok([]);

        return BaseResponse<IEnumerable<AssignmentResponse>>.Ok(assignments.Select(MapToResponse));
    }

    public async Task<BaseResponse<IEnumerable<LearnerAssignmentResponse>>> GetMyAssignmentsAsync(string? status)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<IEnumerable<LearnerAssignmentResponse>>.Fail("Unauthorized");

        var enrollments = await _enrollmentRepository.GetByUserIdAsync(_currentUserService.UserId.Value);
        var courseIds = enrollments.Select(x => x.CourseId).ToHashSet();

        if (courseIds.Count == 0)
            return BaseResponse<IEnumerable<LearnerAssignmentResponse>>.Ok([]);

        var assignments = (await _assignmentRepository.GetAllAsync())
            .Where(x => courseIds.Contains(x.CourseId))
            .OrderBy(x => x.DueDate)
            .ToList();

        var submissions = await _submissionRepository.GetByUserIdAsync(_currentUserService.UserId.Value);
        var submissionsByAssignmentId = submissions.ToDictionary(x => x.AssignmentId, x => x);

        var normalizedStatus = status?.Trim();

        var items = assignments
            .Select(assignment =>
            {
                submissionsByAssignmentId.TryGetValue(assignment.Id, out var submission);

                var assignmentStatus = submission == null
                    ? "Pending"
                    : submission.Score.HasValue
                        ? "Graded"
                        : "Submitted";

                return new LearnerAssignmentResponse(
                    assignment.Id,
                    assignment.CourseId,
                    assignment.Course?.Title ?? "Unknown Course",
                    assignment.Title,
                    assignment.Description,
                    assignment.DueDate,
                    assignmentStatus,
                    assignment.IsPastDue(),
                    submission?.Id,
                    submission?.Answer,
                    submission?.AttachmentUrl,
                    submission?.AttachmentName,
                    submission?.AttachmentContentType,
                    submission?.AttachmentSizeBytes,
                    submission?.SubmittedAt,
                    submission?.Score,
                    submission?.Feedback
                );
            })
            .Where(item => string.IsNullOrWhiteSpace(normalizedStatus) ||
                           string.Equals(item.Status, normalizedStatus, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return BaseResponse<IEnumerable<LearnerAssignmentResponse>>.Ok(items);
    }

    private static AssignmentResponse MapToResponse(Assignment assignment)
    {
        return new AssignmentResponse(
            assignment.Id,
            assignment.CourseId,
            assignment.Course?.Title ?? "Unknown Course",
            assignment.Title,
            assignment.Description,
            assignment.DueDate,
            assignment.IsPastDue()
        );
    }
}
