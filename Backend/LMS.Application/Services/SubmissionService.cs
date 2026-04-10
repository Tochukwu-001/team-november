using LMS.Application.Common;
using LMS.Application.Common.Options;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Assignment;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SubmissionService> _logger;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        ILogger<SubmissionService> logger,
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
    }

    public async Task<BaseResponse<SubmissionResponse>> SubmitAsync(SubmitAssignmentRequest request)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<SubmissionResponse>.Fail("Unauthorized");

        var userId = _currentUserService.UserId.Value;

        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
            return BaseResponse<SubmissionResponse>.Fail("Assignment not found");

        if (assignment.IsPastDue())
            return BaseResponse<SubmissionResponse>.Fail("Assignment due date has passed");

        if (string.IsNullOrWhiteSpace(request.Answer))
            return BaseResponse<SubmissionResponse>.Fail("Add a response or upload a file before submitting");

        var existing = await _submissionRepository
            .GetByAssignmentAndUserAsync(request.AssignmentId, userId);

        if (existing != null)
            return BaseResponse<SubmissionResponse>.Fail("You have already submitted this assignment");

        var submission = Submission.Create(request.AssignmentId, userId, request.Answer);

        await _submissionRepository.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<SubmissionResponse>.Ok(MapToResponse(submission));
    }

    public async Task<BaseResponse<SubmissionResponse>> SubmitWithAttachmentAsync(
        Guid assignmentId,
        string? answer,
        FileUploadRequest request,
        long fileSizeBytes,
        CancellationToken cancellationToken = default)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<SubmissionResponse>.Fail("Unauthorized");

        var userId = _currentUserService.UserId.Value;

        var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
        if (assignment == null)
            return BaseResponse<SubmissionResponse>.Fail("Assignment not found");

        if (assignment.IsPastDue())
            return BaseResponse<SubmissionResponse>.Fail("Assignment due date has passed");

        var existing = await _submissionRepository.GetByAssignmentAndUserAsync(assignmentId, userId);
        if (existing != null)
            return BaseResponse<SubmissionResponse>.Fail("You have already submitted this assignment");

        if (string.IsNullOrWhiteSpace(answer) && fileSizeBytes <= 0)
            return BaseResponse<SubmissionResponse>.Fail("Add a response or upload a file before submitting");

        var submission = Submission.Create(assignmentId, userId, answer);

        if (fileSizeBytes > 0)
        {
            FileUploadResult uploadResult;
            try
            {
                var uploadRequest = request with { Folder = _fileStorageOptions.SubmissionAttachmentFolder };
                uploadResult = await _fileStorageService.UploadDocumentAsync(uploadRequest, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BaseResponse<SubmissionResponse>.Fail(ex.Message);
            }

            submission.AttachFile(uploadResult.Url, uploadResult.OriginalFileName, uploadResult.ContentType, fileSizeBytes);
        }

        await _submissionRepository.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<SubmissionResponse>.Ok(MapToResponse(submission));
    }

    public async Task<BaseResponse<SubmissionResponse>> GradeAsync(Guid submissionId, decimal score, string? feedback)
    {
        var submission = await _submissionRepository.GetByIdAsync(submissionId);

        if (submission == null)
            return BaseResponse<SubmissionResponse>.Fail("Submission not found");

        try
        {
            submission.Grade(score, feedback);
        }
        catch (Exception ex)
        {
            return BaseResponse<SubmissionResponse>.Fail(ex.Message);
        }

        await _submissionRepository.UpdateAsync(submission);
        await _unitOfWork.SaveChangesAsync();
        await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
            submission.UserId,
            LMS.Domain.Enums.NotificationType.AssignmentGraded,
            "Assignment Graded",
            $"{submission.Assignment.Title} has been graded. Score: {submission.Score:0.##}/100.",
            "/assignments"
        ));

        return BaseResponse<SubmissionResponse>.Ok(MapToResponse(submission), "Submission graded successfully");
    }

    private static SubmissionResponse MapToResponse(Submission submission)
    {
        return new SubmissionResponse(
            submission.Id,
            submission.AssignmentId,
            submission.UserId,
            submission.Answer,
            submission.AttachmentUrl,
            submission.AttachmentName,
            submission.AttachmentContentType,
            submission.AttachmentSizeBytes,
            submission.Score,
            submission.Feedback,
            submission.SubmittedAt,
            submission.Score.HasValue
        );
    }

}
