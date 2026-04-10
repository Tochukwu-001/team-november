using FluentValidation;
using LMS.Application.DTOs.Role;

namespace LMS.Application.Validators.Role;

public class ReviewRoleRequestValidator : AbstractValidator<ReviewRoleRequest>
{
    public ReviewRoleRequestValidator()
    {
        RuleFor(x => x.RoleRequestId)
            .NotEmpty();

        RuleFor(x => x.RejectionReason)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.RejectionReason));
    }
}
