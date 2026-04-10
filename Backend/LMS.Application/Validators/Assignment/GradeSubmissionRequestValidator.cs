using FluentValidation;
using LMS.Application.DTOs.Assignment;

namespace LMS.Application.Validators.Assignment;

public class GradeSubmissionRequestValidator : AbstractValidator<GradeSubmissionRequest>
{
    public GradeSubmissionRequestValidator()
    {
        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100);

        RuleFor(x => x.Feedback)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Feedback));
    }
}
