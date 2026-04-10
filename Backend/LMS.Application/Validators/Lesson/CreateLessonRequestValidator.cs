using FluentValidation;
using LMS.Application.DTOs.Lesson;

namespace LMS.Application.Validators.Lesson;

public class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
{
    public CreateLessonRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.EstimatedMinutes)
            .GreaterThan(0)
            .When(x => x.EstimatedMinutes.HasValue);

        RuleFor(x => x.Order)
            .GreaterThan(0);
    }
}
