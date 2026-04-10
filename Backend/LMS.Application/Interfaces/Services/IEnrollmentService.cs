using LMS.Application.Common;
using LMS.Application.DTOs.Enrollment;

namespace LMS.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<BaseResponse<EnrollmentResponse>> EnrollUserAsync(EnrollRequest request);
}
