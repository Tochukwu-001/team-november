namespace LMS.Application.Common.Options;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string Provider { get; set; } = "Cloudinary";
    public string CloudName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string ProfileImageFolder { get; set; } = "talentflow/profile-images";
    public string CourseThumbnailFolder { get; set; } = "talentflow/course-thumbnails";
    public string LessonDocumentFolder { get; set; } = "talentflow/lesson-documents";
    public string SubmissionAttachmentFolder { get; set; } = "talentflow/submission-attachments";
}
