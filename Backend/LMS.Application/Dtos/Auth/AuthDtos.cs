using LMS.Domain.Enums;

namespace LMS.Application.DTOs.Auth;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Discipline,
    string Password,
    string ConfirmPassword
);

public record LoginRequest(string Email, string Password);

public record GoogleSignInRequest(string IdToken, string? Discipline = null);

public record AuthResponse(
    Guid Id,
    string PublicId,
    string FirstName,
    string LastName,
    string Email,
    string Discipline,
    Guid? TeamId,
    string? TeamName,
    UserRole Role,
    string Token
);

public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string Password, string ConfirmPassword);
