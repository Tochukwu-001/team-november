using LMS.Application.DTOs.Course;

namespace LMS.Application.Interfaces.Repositories;

public interface ICourseBrowseRepository
{
    Task<IEnumerable<CourseCatalogItemResponse>> GetCatalogAsync(Guid? userId, string? search, string? category, bool enrolledOnly);
    Task<CourseDetailResponse?> GetCourseDetailAsync(Guid? userId, Guid courseId);
}
