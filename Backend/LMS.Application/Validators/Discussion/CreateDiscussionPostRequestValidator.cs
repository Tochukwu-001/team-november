using FluentValidation;
using LMS.Application.DTOs.Discussion;

namespace LMS.Application.Validators.Discussion;

public class CreateDiscussionPostRequestValidator : AbstractValidator<CreateDiscussionPostRequest>
{
    public CreateDiscussionPostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.Tags)
            .NotNull()
            .Must(tags => tags.Count > 0 && tags.Count <= 5)
            .WithMessage("Provide between 1 and 5 tags");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(50);
    }
}
