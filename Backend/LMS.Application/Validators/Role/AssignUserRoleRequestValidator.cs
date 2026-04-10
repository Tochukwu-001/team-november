using FluentValidation;
using LMS.Application.DTOs.Role;
using LMS.Domain.Enums;

namespace LMS.Application.Validators.Role;

public class AssignUserRoleRequestValidator : AbstractValidator<AssignUserRoleRequest>
{
    public AssignUserRoleRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Role)
            .Must(role => Enum.IsDefined(typeof(UserRole), role))
            .WithMessage("Invalid role");
    }
}
