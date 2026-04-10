using LMS.Domain.Enums;
namespace LMS.Domain.Entities;
public class LessonContent : BaseEntity
{
    public Guid LessonId { get; private set; }
    public Lesson Lesson { get; private set; } = default!; // 🔥 navigation

    public ContentType ContentType { get; private set; }

    public string? Title { get; private set; }
    public string? Url { get; private set; }
    public string? TextContent { get; private set; }

    private LessonContent() { } // For EF Core

    public static LessonContent CreateVideo(Guid lessonId, string url, string? title = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("Lesson is required");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Video URL is required");

        return new LessonContent
        {
            LessonId = lessonId,
            ContentType = ContentType.Video,
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            Url = url.Trim()
        };
    }

    public static LessonContent CreatePdf(Guid lessonId, string url, string? title = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("Lesson is required");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("PDF URL is required");

        return new LessonContent
        {
            LessonId = lessonId,
            ContentType = ContentType.Pdf,
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            Url = url.Trim()
        };
    }

    public static LessonContent CreateText(Guid lessonId, string text, string? title = null)
    {
        if (lessonId == Guid.Empty)
            throw new ArgumentException("Lesson is required");

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text content is required");

        return new LessonContent
        {
            LessonId = lessonId,
            ContentType = ContentType.Text,
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            TextContent = text
        };
    }

    public void UpdateTitle(string? title)
    {
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        SetUpdated();
    }

    public void UpdateText(string text)
    {
        if (ContentType != ContentType.Text)
            throw new InvalidOperationException("Not a text content");

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text content is required");

        TextContent = text;
        SetUpdated();
    }

    public void UpdateUrl(string url)
    {
        if (ContentType == ContentType.Text)
            throw new InvalidOperationException("Text content does not use URL");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required");

        Url = url.Trim();
        SetUpdated();
    }
}
