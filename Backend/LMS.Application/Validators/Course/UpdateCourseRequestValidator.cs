using FluentValidation;
using LMS.Application.DTOs.Course;

namespace LMS.Application.Validators.Course;

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(x => x.Category)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl))
            .WithMessage("Thumbnail URL must be a valid absolute URL");
    }

    private static bool BeAValidUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }
}
