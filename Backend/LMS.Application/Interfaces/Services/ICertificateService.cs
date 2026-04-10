using LMS.Application.Common;
using LMS.Application.DTOs.Certificate;

namespace LMS.Application.Interfaces.Services;

public interface ICertificateService
{
    Task<BaseResponse<IEnumerable<CertificateResponse>>> GetMyCertificatesAsync();
    Task<BaseResponse<CertificateResponse>> IssueCertificateAsync(Guid courseId);
}
