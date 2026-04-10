using LMS.Application.Common;
using LMS.Application.DTOs.Enrollment;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IProgressRepository _progressRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository, 
        ICourseRepository courseRepository,
        IProgressRepository progressRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _progressRepository = progressRepository;
    }

    public async Task<BaseResponse<EnrollmentResponse>> EnrollUserAsync(EnrollRequest request)
    {
        var existing = await _enrollmentRepository.GetByUserAndCourseAsync(request.UserId, request.CourseId);
        if (existing != null) return BaseResponse<EnrollmentResponse>.Fail("User already enrolled");

        var course = await _courseRepository.GetByIdAsync(request.CourseId);
        if (course == null) return BaseResponse<EnrollmentResponse>.Fail("Course not found");

        var enrollment = Enrollment.Create(request.UserId, request.CourseId);
        await _enrollmentRepository.AddAsync(enrollment);

        // Initialize Progress
        var progress = Progress.Create(request.UserId, request.CourseId, course.LessonCount);
        await _progressRepository.AddAsync(progress);

        var response = new EnrollmentResponse(
            enrollment.Id,
            enrollment.UserId,
            enrollment.CourseId,
            enrollment.EnrolledAt,
            enrollment.IsCompleted
        );

        return BaseResponse<EnrollmentResponse>.Ok(response, "Enrolled successfully");
    }
}
