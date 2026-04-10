using LMS.Application.Common;
using LMS.Application.Common.Auth;
using LMS.Application.Common.Options;
using LMS.Application.DTOs.Auth;
using LMS.Application.Email;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace LMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _config;
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly GoogleAuthOptions _googleAuthOptions;

    public AuthService(
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IPasswordResetTokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        INotificationService notificationService,
        IConfiguration config,
        IGoogleTokenValidator googleTokenValidator,
        Microsoft.Extensions.Options.IOptions<GoogleAuthOptions> googleAuthOptions)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _notificationService = notificationService;
        _config = config;
        _googleTokenValidator = googleTokenValidator;
        _googleAuthOptions = googleAuthOptions.Value;
    }
    public async Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var discipline = TeamCatalog.NormalizeDiscipline(request.Discipline);
        var teamName = TeamCatalog.GetTeamNameForDiscipline(discipline);

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null && existingUser.IsActive)
        {
            return BaseResponse<AuthResponse>.Fail("Email already exists");
        }

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var team = await _teamRepository.GetByNameAsync(teamName);

        if (team == null)
        {
            var teamDefinition = TeamCatalog.GetTeamDefinition(teamName);
            team = Team.Create(teamDefinition.Name, teamDefinition.Description);
            await _teamRepository.AddAsync(team);
        }

        var user = User.Create(
            request.FirstName,
            request.LastName,
            email,
            discipline,
            hashedPassword,
            UserRole.Learner,
            team.Id,
            LearnerProfileDefaults.CohortLabel,
            LearnerProfileDefaults.Location
        );

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
            user.Id,
            LMS.Domain.Enums.NotificationType.System,
            "Welcome to TalentFlow",
            "Your account is ready. Explore your courses, team, and upcoming tasks.",
            "/dashboard"
        ));

        await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
            user.Id,
            LMS.Domain.Enums.NotificationType.TeamUpdate,
            "Team Update",
            $"You have been added to the {team.Name} cross-functional team.",
            "/my-team"
        ));

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());

        var response = new AuthResponse(
            user.Id,
            user.PublicId,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Discipline,
            user.TeamId,
            team.Name,
            user.Role,
            token
        );

        return BaseResponse<AuthResponse>.Ok(response, "User registered successfully");
    }

    public async Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            return BaseResponse<AuthResponse>.Fail("Invalid email or password");
        }

        var isValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

        if (!isValid)
        {
            return BaseResponse<AuthResponse>.Fail("Invalid email or password");
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());

        var response = new AuthResponse(
            user.Id,
            user.PublicId,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Discipline,
            user.TeamId,
            user.Team?.Name,
            user.Role,
            token
        );

        return BaseResponse<AuthResponse>.Ok(response, "Login successful");
    }

    public async Task<BaseResponse<AuthResponse>> GoogleSignInAsync(GoogleSignInRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return BaseResponse<AuthResponse>.Fail("Google ID token is required");

        GoogleUserInfo googleUser;
        try
        {
            googleUser = await _googleTokenValidator.ValidateIdTokenAsync(request.IdToken, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BaseResponse<AuthResponse>.Fail(ex.Message);
        }

        var email = googleUser.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            if (!_googleAuthOptions.AllowJustInTimeRegistration)
                return BaseResponse<AuthResponse>.Fail("No account was found for this Google user");

            if (string.IsNullOrWhiteSpace(request.Discipline))
                return BaseResponse<AuthResponse>.Fail("Discipline is required to create a new account with Google");

            var discipline = TeamCatalog.NormalizeDiscipline(request.Discipline);
            var teamName = TeamCatalog.GetTeamNameForDiscipline(discipline);
            var team = await _teamRepository.GetByNameAsync(teamName);

            if (team == null)
            {
                var teamDefinition = TeamCatalog.GetTeamDefinition(teamName);
                team = Team.Create(teamDefinition.Name, teamDefinition.Description);
                await _teamRepository.AddAsync(team);
            }

            var generatedPassword = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
            user = User.Create(
                googleUser.GivenName ?? "Google",
                googleUser.FamilyName ?? "User",
                email,
                discipline,
                generatedPassword,
                UserRole.Learner,
                team.Id,
                LearnerProfileDefaults.CohortLabel,
                LearnerProfileDefaults.Location
            );

            if (!string.IsNullOrWhiteSpace(googleUser.PictureUrl))
                user.UpdateProfileImage(googleUser.PictureUrl);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
                user.Id,
                LMS.Domain.Enums.NotificationType.System,
                "Welcome to TalentFlow",
                "Your Google account has been linked successfully. Explore your courses, team, and upcoming tasks.",
                "/dashboard"
            ));

            await _notificationService.NotifyUserAsync(new LMS.Application.DTOs.Notification.CreateNotificationRequest(
                user.Id,
                LMS.Domain.Enums.NotificationType.TeamUpdate,
                "Team Update",
                $"You have been added to the {team.Name} cross-functional team.",
                "/my-team"
            ));
        }
        else if (!user.IsActive)
        {
            return BaseResponse<AuthResponse>.Fail("This account is inactive");
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.Role.ToString());

        var response = new AuthResponse(
            user.Id,
            user.PublicId,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Discipline,
            user.TeamId,
            user.Team?.Name,
            user.Role,
            token
        );

        return BaseResponse<AuthResponse>.Ok(response, "Google sign-in successful");
    }

    public async Task<BaseResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
            return BaseResponse<string>.Ok("If the email exists, a reset link has been sent");

        await _tokenRepository.InvalidateUserTokensAsync(user.Id);

        var rawToken = Guid.NewGuid().ToString();

        var tokenHash = _passwordHasher.HashPassword(rawToken);

        var resetToken = PasswordResetToken.Create(
            user.Id,
            tokenHash,
            DateTime.UtcNow.AddMinutes(15)
        );

        await _tokenRepository.AddAsync(resetToken);
        await _unitOfWork.SaveChangesAsync();

        var frontendUrl = _config["App:FrontendUrl"];

        var resetLink = $"{frontendUrl}/reset-password?token={rawToken}";

        var html = EmailTemplates.PasswordReset(resetLink);

        await _emailService.SendEmailAsync(
            user.Email,
            "Reset your password",
            html
        );

        return BaseResponse<string>.Ok("If the email exists, a reset link has been sent");
    }

    public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return BaseResponse<string>.Fail("Passwords do not match");

        var tokens = await _tokenRepository.GetValidTokensAsync();

        PasswordResetToken? matchedToken = null;

        foreach (var token in tokens)
        {
            if (_passwordHasher.VerifyPassword(request.Token, token.TokenHash))
            {
                matchedToken = token;
                break;
            }
        }

        if (matchedToken == null)
            return BaseResponse<string>.Fail("Invalid or expired token");

        var user = matchedToken.User;

        if (user == null || !user.IsActive)
            return BaseResponse<string>.Fail("User not found");

        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        user.SetPassword(hashedPassword);

        matchedToken.MarkAsUsed();

        await _unitOfWork.SaveChangesAsync();

        return BaseResponse<string>.Ok("Password reset successful");
    }
}
