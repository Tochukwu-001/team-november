namespace LMS.Application.Common.Auth;

public sealed record GoogleUserInfo(
    string Subject,
    string Email,
    string? GivenName,
    string? FamilyName,
    string? PictureUrl,
    string? HostedDomain
);
