using LMS.Application.Common;
using LMS.Application.DTOs.Auth;

namespace LMS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<BaseResponse<AuthResponse>> GoogleSignInAsync(GoogleSignInRequest request, CancellationToken cancellationToken = default);
    Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
}
