using LMS.Application.Common;
using LMS.Application.DTOs.Certificate;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationService _notificationService;

    public CertificateService(
        ICertificateRepository certificateRepository,
        ISubmissionRepository submissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        INotificationService notificationService)
    {
        _certificateRepository = certificateRepository;
        _submissionRepository = submissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<BaseResponse<IEnumerable<CertificateResponse>>> GetMyCertificatesAsync()
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<IEnumerable<CertificateResponse>>.Fail("Unauthorized");

        var userId = _currentUserService.UserId.Value;

        var certificates = await _certificateRepository.GetByUserIdAsync(userId);

        return BaseResponse<IEnumerable<CertificateResponse>>.Ok(
            certificates
                .OrderByDescending(item => item.IssuedAt)
                .Select(MapToResponse));
    }

    public async Task<BaseResponse<CertificateResponse>> IssueCertificateAsync(Guid courseId)
    {
        if (_currentUserService.UserId == null)
            return BaseResponse<CertificateResponse>.Fail("Unauthorized");

        var userId = _currentUserService.UserId.Value;

        var existing = await _certificateRepository.GetByUserAndCourseAsync(userId, courseId);
        if (existing != null)
            return BaseResponse<CertificateResponse>.Fail("Certificate already issued for this course");

        var submissions = await _submissionRepository.GetByUserIdAsync(userId);

        var courseSubmissions = submissions
            .Where(s => s.Assignment.CourseId == courseId)
            .ToList();

        if (!courseSubmissions.Any())
            return BaseResponse<CertificateResponse>.Fail("No submissions found for this course");

        if (courseSubmissions.Any(s => !s.Score.HasValue))
            return BaseResponse<CertificateResponse>.Fail("All assignments must be graded before issuing certificate");

        var finalScore = courseSubmissions.Average(s => s.Score!.Value);
        var courseTitle = courseSubmissions.First().Assignment.Course?.Title ?? "your course";

        if (finalScore < 50)
            return BaseResponse<CertificateResponse>.Fail("Minimum score not reached for certification");

        var certificate = Certificate.Create(userId, courseId, finalScore);

        await _certificateRepository.AddAsync(certificate);
        await _unitOfWork.SaveChangesAsync();
        await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
            userId,
            LMS.Domain.Enums.NotificationType.CertificateMilestone,
            "Certificate Milestone",
            $"You earned a certificate for completing {courseTitle}.",
            "/certificates"
        ));

        return BaseResponse<CertificateResponse>.Ok(MapToResponse(certificate), "Certificate issued successfully");
    }

    private static CertificateResponse MapToResponse(Certificate certificate)
    {
        return new CertificateResponse(
            certificate.Id,
            certificate.UserId,
            certificate.CourseId,
            certificate.Course?.Title ?? "Unknown",
            certificate.User?.FullName ?? "Unknown",
            certificate.User?.PublicId ?? "Unknown",
            certificate.User?.Discipline ?? "Unknown",
            certificate.User?.CohortLabel,
            certificate.FinalScore,
            certificate.CertificateNumber,
            certificate.IssuedAt
        );
    }
}
