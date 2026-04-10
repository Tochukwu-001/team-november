using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Lesson;

namespace LMS.Application.Interfaces.Services;

public interface ILessonService
{
    Task<BaseResponse<LessonResponse>> CreateLessonAsync(CreateLessonRequest request);
    Task<BaseResponse<LessonResponse>> AddContentAsync(AddLessonContentRequest request);
    Task<BaseResponse<IEnumerable<LessonResponse>>> GetLessonsByCourseIdAsync(Guid courseId);
    Task<BaseResponse<LessonPlayerResponse>> GetLessonPlayerAsync(Guid lessonId);
    Task<BaseResponse<bool>> CompleteLessonAsync(Guid lessonId);
    Task<BaseResponse<LessonNoteResponse>> SaveLessonNoteAsync(Guid lessonId, SaveLessonNoteRequest request);
    Task<BaseResponse<LessonResponse>> UploadPdfContentAsync(Guid lessonId, FileUploadRequest request, CancellationToken cancellationToken = default);
}
