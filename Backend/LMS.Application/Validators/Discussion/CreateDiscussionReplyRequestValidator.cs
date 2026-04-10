using FluentValidation;
using LMS.Application.DTOs.Discussion;

namespace LMS.Application.Validators.Discussion;

public class CreateDiscussionReplyRequestValidator : AbstractValidator<CreateDiscussionReplyRequest>
{
    public CreateDiscussionReplyRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(3000);
    }
}
