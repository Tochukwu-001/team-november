namespace LMS.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlContent);
    Task SendNotificationEmailAsync(string to, string recipientName, string title, string message, string? actionUrl = null);
}
