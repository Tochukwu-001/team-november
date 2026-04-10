namespace LMS.Application.Common.Storage;

public sealed record FileUploadRequest(
    Stream Content,
    string FileName,
    string ContentType,
    string Folder
);
