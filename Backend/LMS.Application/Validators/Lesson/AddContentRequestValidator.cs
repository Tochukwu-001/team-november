using FluentValidation;
using LMS.Application.DTOs.Lesson;
using LMS.Domain.Enums;

namespace LMS.Application.Validators.Lesson;

public class AddContentRequestValidator : AbstractValidator<AddLessonContentRequest>
{
    public AddContentRequestValidator()
    {
        RuleFor(x => x.LessonId)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .IsInEnum()
            .WithMessage("Invalid content type");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.ContentUrl)
            .Must(url => !string.IsNullOrWhiteSpace(url))
            .When(x => x.ContentType != ContentType.Text)
            .WithMessage("URL is required for Video and PDF content");

        RuleFor(x => x.ContentUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => x.ContentType != ContentType.Text && !string.IsNullOrWhiteSpace(x.ContentUrl))
            .WithMessage("Invalid URL format");

        RuleFor(x => x.TextContent)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .When(x => x.ContentType == ContentType.Text)
            .WithMessage("Text content is required for Text type");

    }
}
