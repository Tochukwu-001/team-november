using FluentValidation;
using LMS.Application.DTOs.Assignment;

namespace LMS.Application.Validators.Assignment;

public class SubmitAssignmentRequestValidator : AbstractValidator<SubmitAssignmentRequest>
{
    public SubmitAssignmentRequestValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty().WithMessage("Assignment is required");

        RuleFor(x => x.AssignmentId)
                    .NotEmpty().WithMessage("Assignment is required");

        RuleFor(x => x.Answer)
            .NotEmpty()
            .Must(a => !string.IsNullOrWhiteSpace(a))
            .WithMessage("Answer cannot be empty")
            .MaximumLength(5000).WithMessage("Answer must not exceed 5000 characters");
    }
}
