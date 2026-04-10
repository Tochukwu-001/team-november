using LMS.Application.Common;
using LMS.Application.DTOs.Progress;

namespace LMS.Application.Interfaces.Services;

public interface IProgressService
{
    Task<BaseResponse<ProgressResponse>> GetProgressAsync(Guid userId, Guid courseId);
    Task<BaseResponse<bool>> CompleteLessonAsync(CompleteLessonRequest request);
    Task<BaseResponse<ProgressOverviewResponse>> GetMyProgressAsync();
}
