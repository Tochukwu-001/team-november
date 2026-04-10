using FluentValidation;
using LMS.Application.DTOs.Lesson;

namespace LMS.Application.Validators.Lesson;

public class SaveLessonNoteRequestValidator : AbstractValidator<SaveLessonNoteRequest>
{
    public SaveLessonNoteRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
