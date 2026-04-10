using FluentValidation;
using LMS.Application.Common;
using LMS.Application.DTOs.Auth;
namespace LMS.Application.Validators.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
<<<<<<< HEAD
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
       
        RuleFor(x => x.LastName)
           .NotEmpty().WithMessage("Last name is required")
           .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
=======
            .MaximumLength(100)
            .Matches("^[a-zA-Z]+$").WithMessage("First name must contain only letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100)
            .Matches("^[a-zA-Z]+$").WithMessage("Last name must contain only letters");
>>>>>>> origin/master

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
<<<<<<< HEAD
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
=======

        RuleFor(x => x.Discipline)
            .NotEmpty().WithMessage("Discipline is required")
            .MaximumLength(100)
            .Must(TeamCatalog.IsSupportedDiscipline)
            .WithMessage("Please select a valid discipline");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
>>>>>>> origin/master
    }
}
