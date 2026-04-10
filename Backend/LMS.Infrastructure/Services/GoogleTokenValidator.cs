using Google.Apis.Auth;
using LMS.Application.Common.Auth;
using LMS.Application.Common.Options;
using LMS.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options;

    public GoogleTokenValidator(IOptions<GoogleAuthOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GoogleUserInfo> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId))
            throw new InvalidOperationException("Google authentication is not configured");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_options.ClientId]
            });
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Invalid Google ID token");
        }

        if (!string.IsNullOrWhiteSpace(_options.HostedDomain) &&
            !string.Equals(payload.HostedDomain, _options.HostedDomain, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Google account is not part of the allowed organization");
        }

        if (string.IsNullOrWhiteSpace(payload.Email))
            throw new InvalidOperationException("Google account email is missing");

        return new GoogleUserInfo(
            payload.Subject,
            payload.Email,
            payload.GivenName,
            payload.FamilyName,
            payload.Picture,
            payload.HostedDomain
        );
    }
}
