using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Course;

namespace LMS.Application.Interfaces.Services;

public interface ICourseService
{
    Task<BaseResponse<CourseResponse>> CreateCourseAsync(CreateCourseRequest request);
    Task<BaseResponse<CourseResponse>> UpdateCourseAsync(Guid id, UpdateCourseRequest request);
    Task<BaseResponse<CourseResponse>> GetCourseByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<CourseResponse>>> GetAllCoursesAsync();
    Task<BaseResponse<IEnumerable<CourseCatalogItemResponse>>> GetCatalogAsync(string? search, string? category, bool enrolledOnly);
    Task<BaseResponse<CourseDetailResponse>> GetCourseDetailAsync(Guid courseId);
    Task<BaseResponse<CourseResponse>> UploadThumbnailAsync(Guid courseId, FileUploadRequest request, CancellationToken cancellationToken = default);
}
