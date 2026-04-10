using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LMS.Application.Common.Options;
using LMS.Application.Common.Storage;
using LMS.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMS.Infrastructure.Services;

public class CloudinaryFileStorageService : IFileStorageService
{
    private readonly HttpClient _httpClient;
    private readonly FileStorageOptions _options;
    private readonly ILogger<CloudinaryFileStorageService> _logger;

    public CloudinaryFileStorageService(
        HttpClient httpClient,
        IOptions<FileStorageOptions> options,
        ILogger<CloudinaryFileStorageService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<FileUploadResult> UploadImageAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        return UploadAsync(request, "image", cancellationToken);
    }

    public Task<FileUploadResult> UploadDocumentAsync(FileUploadRequest request, CancellationToken cancellationToken = default)
    {
        return UploadAsync(request, "raw", cancellationToken);
    }

    private async Task<FileUploadResult> UploadAsync(
        FileUploadRequest request,
        string resourceType,
        CancellationToken cancellationToken)
    {
        ValidateConfiguration();

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var folder = string.IsNullOrWhiteSpace(request.Folder) ? "talentflow" : request.Folder.Trim();
        var signature = CreateSignature(folder, timestamp);
        var endpoint = $"https://api.cloudinary.com/v1_1/{_options.CloudName}/{resourceType}/upload";

        using var formData = new MultipartFormDataContent();
        using var streamContent = new StreamContent(request.Content);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);

        formData.Add(streamContent, "file", request.FileName);
        formData.Add(new StringContent(folder), "folder");
        formData.Add(new StringContent(timestamp.ToString()), "timestamp");
        formData.Add(new StringContent(_options.ApiKey), "api_key");
        formData.Add(new StringContent(signature), "signature");

        using var response = await _httpClient.PostAsync(endpoint, formData, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Cloudinary upload failed. Status: {StatusCode}. Payload: {Payload}", response.StatusCode, payload);
            throw new InvalidOperationException("File upload failed");
        }

        var uploadResponse = JsonSerializer.Deserialize<CloudinaryUploadResponse>(payload);
        if (uploadResponse == null || string.IsNullOrWhiteSpace(uploadResponse.SecureUrl) || string.IsNullOrWhiteSpace(uploadResponse.PublicId))
            throw new InvalidOperationException("File upload returned an invalid response");

        return new FileUploadResult(
            uploadResponse.SecureUrl,
            uploadResponse.PublicId,
            request.FileName,
            request.ContentType
        );
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.CloudName) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.ApiSecret))
        {
            throw new InvalidOperationException("File storage is not configured");
        }
    }

    private string CreateSignature(string folder, long timestamp)
    {
        var signaturePayload = $"folder={folder}&timestamp={timestamp}{_options.ApiSecret}";
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(signaturePayload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class CloudinaryUploadResponse
    {
        [JsonPropertyName("secure_url")]
        public string SecureUrl { get; set; } = string.Empty;

        [JsonPropertyName("public_id")]
        public string PublicId { get; set; } = string.Empty;
    }
}
