namespace LMS.Application.Common.Options;

public class GoogleAuthOptions
{
    public const string SectionName = "GoogleAuth";

    public string ClientId { get; set; } = string.Empty;
    public string? HostedDomain { get; set; }
    public bool AllowJustInTimeRegistration { get; set; }
}
