namespace LMS.Application.Common.Storage;

public sealed record FileUploadResult(
    string Url,
    string PublicId,
    string OriginalFileName,
    string ContentType
);
