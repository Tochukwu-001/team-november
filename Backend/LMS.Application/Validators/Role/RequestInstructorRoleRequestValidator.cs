using FluentValidation;
using LMS.Application.DTOs.Role;

namespace LMS.Application.Validators.Role;

public class RequestInstructorRoleRequestValidator : AbstractValidator<RequestInstructorRoleRequest>
{
    public RequestInstructorRoleRequestValidator()
    {
        RuleFor(x => x.Bio)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Expertise)
            .NotEmpty()
            .MaximumLength(500);
    }
}
