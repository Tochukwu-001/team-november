using LMS.Application.Common.Storage;

namespace LMS.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadImageAsync(FileUploadRequest request, CancellationToken cancellationToken = default);
    Task<FileUploadResult> UploadDocumentAsync(FileUploadRequest request, CancellationToken cancellationToken = default);
}
