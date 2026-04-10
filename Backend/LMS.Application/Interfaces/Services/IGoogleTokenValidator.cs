using LMS.Application.Common.Auth;

namespace LMS.Application.Interfaces.Services;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
