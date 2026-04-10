using FluentValidation;
using LMS.Application.DTOs.Profile;

namespace LMS.Application.Validators.Profile;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100)
            .Matches("^[a-zA-Z]+$").WithMessage("First name must contain only letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100)
            .Matches("^[a-zA-Z]+$").WithMessage("Last name must contain only letters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .Matches(@"^\+?[0-9\s\-\(\)]*$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Phone number format is invalid");
    }
}
