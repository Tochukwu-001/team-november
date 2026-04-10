using LMS.Application.Common;
using LMS.Application.Common.Options;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Lesson;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.Extensions.Options;

namespace LMS.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly ILessonNoteRepository _lessonNoteRepository;
    private readonly ICourseRepository _courseRepository;

    private readonly IProgressRepository _progressRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageOptions _fileStorageOptions;

    public LessonService(
        ILessonRepository lessonRepository,
        ILessonProgressRepository lessonProgressRepository,
        ILessonNoteRepository lessonNoteRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IProgressRepository progressRepository,
        ICurrentUserService currentUserService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IOptions<FileStorageOptions> fileStorageOptions)
   
    {
        _lessonRepository = lessonRepository;
        _lessonProgressRepository = lessonProgressRepository;
        _lessonNoteRepository = lessonNoteRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _progressRepository = progressRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _fileStorageOptions = fileStorageOptions.Value;
    }

    public async Task<BaseResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest request)
    {
        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null) return BaseResponse<LessonResponse>.Fail("Course not found");

        var lesson = Lesson.Create(request.CourseId, request.Title, request.Order, request.Description, request.EstimatedMinutes);
        await _lessonRepository.AddAsync(lesson);
        await _unitOfWork.SaveChangesAsync();

        var enrollments = await _enrollmentRepository.GetByCourseIdAsync(request.CourseId);
        await _notificationService.NotifyUsersAsync(enrollments.Select(enrollment =>
            new LMS.Application.DTOs.Notification.CreateNotificationRequest(
                enrollment.UserId,
                LMS.Domain.Enums.NotificationType.LessonAvailable,
                "New Lesson Available",
                $"A new lesson, {lesson.Title}, has been added to {course.Title}.",
                $"/courses/{course.Id}"
            )));

        return BaseResponse<LessonResponse>.Ok(MapToResponse(lesson), "Lesson created successfully");
    }

    public async Task<BaseResponse<LessonResponse>> AddContentAsync(AddLessonContentRequest request)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
        if (lesson == null)
        {
            return BaseResponse<LessonResponse>.Fail("Lesson not found");

        }

        if (request.ContentType != ContentType.Text && string.IsNullOrWhiteSpace(request.ContentUrl))
        {
            return BaseResponse<LessonResponse>.Fail("Content URL is required");
        }

        if (request.ContentType == ContentType.Text && string.IsNullOrWhiteSpace(request.TextContent))
        {
            return BaseResponse<LessonResponse>.Fail("Text content is required");
        }

        LessonContent content = request.ContentType switch
        {
            ContentType.Video => LessonContent.CreateVideo(request.LessonId, request.ContentUrl!, request.Title),
            ContentType.Pdf => LessonContent.CreatePdf(request.LessonId, request.ContentUrl!, request.Title),
            ContentType.Text => LessonContent.CreateText(request.LessonId, request.TextContent!, request.Title),
            _ => throw new ArgumentException("Invalid content type")
        };

        lesson.AddContent(content);
        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<LessonResponse>.Ok(MapToResponse(lesson), "Content added successfully");
    }

    public async Task<BaseResponse<IEnumerable<LessonResponse>>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId);
        if (lessons.Count == 0)
        {
            return BaseResponse<IEnumerable<LessonResponse>>.Fail("No lessons found for this course");
        }
        var response = lessons.Select(MapToResponse).ToList();
        return BaseResponse<IEnumerable<LessonResponse>>.Ok(response);
    }

    public async Task<BaseResponse<LessonPlayerResponse>> GetLessonPlayerAsync(Guid lessonId)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<LessonPlayerResponse>.Fail("User is not authenticated");

        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return BaseResponse<LessonPlayerResponse>.Fail("Lesson not found");

        var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(_currentUserService.UserId.Value, lesson.CourseId);
        if (enrollment == null)
            return BaseResponse<LessonPlayerResponse>.Fail("You must enroll in this course to access lessons");

        var course = await _courseRepository.GetByIdAsync(lesson.CourseId);
        if (course == null)
            return BaseResponse<LessonPlayerResponse>.Fail("Course not found");

        var orderedLessons = (await _lessonRepository.GetByCourseIdAsync(lesson.CourseId))
            .OrderBy(x => x.Order)
            .ToList();

        var completedLessons = new Dictionary<Guid, bool>();
        foreach (var lessonItem in orderedLessons)
        {
            var progress = await _lessonProgressRepository.GetByUserAndLessonAsync(_currentUserService.UserId.Value, lessonItem.Id);
            completedLessons[lessonItem.Id] = progress?.IsCompleted ?? false;
        }

        var currentIndex = orderedLessons.FindIndex(x => x.Id == lessonId);
        Guid? previousLessonId = currentIndex > 0 ? orderedLessons[currentIndex - 1].Id : null;
        Guid? nextLessonId = currentIndex >= 0 && currentIndex < orderedLessons.Count - 1 ? orderedLessons[currentIndex + 1].Id : null;
        var firstIncompleteLessonId = orderedLessons.FirstOrDefault(x => !completedLessons[x.Id])?.Id;

        var sidebar = orderedLessons
            .Select(item =>
            {
                var isCompleted = completedLessons[item.Id];
                var hasIncompleteBefore = orderedLessons
                    .Where(x => x.Order < item.Order)
                    .Any(x => !completedLessons[x.Id]);

                return new LessonPlayerSidebarItemResponse(
                    item.Id,
                    item.Title,
                    item.Order,
                    item.EstimatedMinutes,
                    isCompleted,
                    hasIncompleteBefore && !isCompleted,
                    item.Id == lessonId || firstIncompleteLessonId == item.Id
                );
            })
            .ToList();

        var progressSummary = await _progressRepository.GetByUserAndCourseAsync(_currentUserService.UserId.Value, lesson.CourseId);
        var note = await _lessonNoteRepository.GetByUserAndLessonAsync(_currentUserService.UserId.Value, lessonId);
        var lessonProgress = await _lessonProgressRepository.GetByUserAndLessonAsync(_currentUserService.UserId.Value, lessonId);

        var response = new LessonPlayerResponse(
            lesson.CourseId,
            course.Title,
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Order,
            Math.Round(progressSummary?.Percentage ?? 0, 1),
            lessonProgress?.IsCompleted ?? false,
            previousLessonId,
            nextLessonId,
            lesson.Contents
                .Select(content => new LessonPlayerContentResponse(content.Id, content.ContentType, content.Title, content.Url, content.TextContent))
                .ToList(),
            sidebar,
            note == null ? null : new LessonNoteResponse(note.Id, note.Content, note.UpdatedAt ?? note.CreatedAt)
        );

        return BaseResponse<LessonPlayerResponse>.Ok(response);
    }

    public async Task<BaseResponse<bool>> CompleteLessonAsync(Guid lessonId)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<bool>.Fail("User is not authenticated");

        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return BaseResponse<bool>.Fail("Lesson not found");

        var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(_currentUserService.UserId.Value, lesson.CourseId);
        if (enrollment == null)
            return BaseResponse<bool>.Fail("You must enroll in this course to complete lessons");

        var lessonProgress = await _lessonProgressRepository.GetByUserAndLessonAsync(_currentUserService.UserId.Value, lessonId);
        if (lessonProgress == null)
        {
            lessonProgress = LessonProgress.Create(_currentUserService.UserId.Value, lessonId);
            await _lessonProgressRepository.AddAsync(lessonProgress);
        }

        if (!lessonProgress.IsCompleted)
        {
            lessonProgress.MarkAsCompleted();
            await _lessonProgressRepository.UpdateAsync(lessonProgress);
        }

        var totalLessons = (await _lessonRepository.GetByCourseIdAsync(lesson.CourseId)).Count;
        var completedLessonsCount = await _lessonProgressRepository.GetCompletedLessonsCountAsync(_currentUserService.UserId.Value, lesson.CourseId);
        var progress = await _progressRepository.GetByUserAndCourseAsync(_currentUserService.UserId.Value, lesson.CourseId);

        if (progress == null)
        {
            progress = Progress.Create(_currentUserService.UserId.Value, lesson.CourseId, totalLessons);
            await _progressRepository.AddAsync(progress);
        }

        progress.UpdateProgress(completedLessonsCount);
        await _progressRepository.UpdateAsync(progress);

        if (progress.Percentage >= 100 && !enrollment.IsCompleted)
        {
            enrollment.MarkAsCompleted();
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        return BaseResponse<bool>.Ok(true, "Lesson completed successfully");
    }

    public async Task<BaseResponse<LessonNoteResponse>> SaveLessonNoteAsync(Guid lessonId, SaveLessonNoteRequest request)
    {
        if (!_currentUserService.UserId.HasValue)
            return BaseResponse<LessonNoteResponse>.Fail("User is not authenticated");

        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return BaseResponse<LessonNoteResponse>.Fail("Lesson not found");

        var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(_currentUserService.UserId.Value, lesson.CourseId);
        if (enrollment == null)
            return BaseResponse<LessonNoteResponse>.Fail("You must enroll in this course to save notes");

        var note = await _lessonNoteRepository.GetByUserAndLessonAsync(_currentUserService.UserId.Value, lessonId);
        if (note == null)
        {
            note = LessonNote.Create(_currentUserService.UserId.Value, lessonId, request.Content);
            await _lessonNoteRepository.AddAsync(note);
        }
        else
        {
            note.UpdateContent(request.Content);
            await _lessonNoteRepository.UpdateAsync(note);
        }

        return BaseResponse<LessonNoteResponse>.Ok(
            new LessonNoteResponse(note.Id, note.Content, note.UpdatedAt ?? note.CreatedAt),
            "Lesson note saved successfully");
    }

    public async Task<BaseResponse<LessonResponse>> UploadPdfContentAsync(
        Guid lessonId,
        FileUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return BaseResponse<LessonResponse>.Fail("Lesson not found");

        var isPdfContentType = string.Equals(request.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
        var hasPdfExtension = Path.GetExtension(request.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);

        if (!isPdfContentType && !hasPdfExtension)
            return BaseResponse<LessonResponse>.Fail("Only PDF files are allowed");

        FileUploadResult uploadResult;
        try
        {
            var uploadRequest = request with { Folder = _fileStorageOptions.LessonDocumentFolder };
            uploadResult = await _fileStorageService.UploadDocumentAsync(uploadRequest, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BaseResponse<LessonResponse>.Fail(ex.Message);
        }

        var content = LessonContent.CreatePdf(lessonId, uploadResult.Url, Path.GetFileNameWithoutExtension(request.FileName));
        lesson.AddContent(content);
        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<LessonResponse>.Ok(MapToResponse(lesson), "PDF content uploaded successfully");
    }

    private LessonResponse MapToResponse(Lesson lesson)
    {
        return new LessonResponse(
            lesson.Id,
            lesson.CourseId,
            lesson.Title,
            lesson.Description,
            lesson.EstimatedMinutes,
            lesson.Order,
            lesson.ContentCount
        );
    }
}
