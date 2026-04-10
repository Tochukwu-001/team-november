using LMS.Application.Common;
using LMS.Application.Common.Storage;
using LMS.Application.DTOs.Assignment;

namespace LMS.Application.Interfaces.Services;

public interface ISubmissionService
{
    Task<BaseResponse<SubmissionResponse>> SubmitAsync(SubmitAssignmentRequest request);
    Task<BaseResponse<SubmissionResponse>> SubmitWithAttachmentAsync(Guid assignmentId, string? answer, FileUploadRequest request, long fileSizeBytes, CancellationToken cancellationToken = default);
    Task<BaseResponse<SubmissionResponse>> GradeAsync(Guid submissionId, decimal score, string? feedback);

    //Task<BaseResponse<IEnumerable<SubmissionResponse>>> GetByAssignmentAsync(Guid assignmentId);

    //Task<BaseResponse<IEnumerable<SubmissionResponse>>> GetMySubmissionsAsync();

    //Task<BaseResponse<SubmissionResponse>> GradeAsync(Guid submissionId, decimal score);
}
