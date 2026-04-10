using System.Net;
using System.Net.Mail;
using LMS.Application.Email;
using LMS.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        var settings = GetRequiredEmailSettings();
        await SendEmailInternalAsync(to, subject, htmlContent, settings);
    }

    public async Task SendNotificationEmailAsync(string to, string recipientName, string title, string message, string? actionUrl = null)
    {
        if (!TryGetEmailSettings(out var settings))
        {
            _logger.LogInformation(
                "Skipping notification email for {Recipient} because email settings are not fully configured.",
                to);
            return;
        }

        var html = EmailTemplates.Notification(recipientName, title, message, actionUrl);
        try
        {
            await SendEmailInternalAsync(to, title, html, settings);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send notification email to {Recipient}. In-app notification was still created.", to);
        }
    }

    private async Task SendEmailInternalAsync(string to, string subject, string htmlContent, EmailSettings settings)
    {
        var smtpClient = new SmtpClient(settings.SmtpHost)
        {
            Port = settings.Port,
            Credentials = new NetworkCredential(settings.Username, settings.Password),
            EnableSsl = settings.EnableSsl
        };

        var mail = new MailMessage
        {
            From = string.IsNullOrWhiteSpace(settings.FromName)
                ? new MailAddress(settings.FromAddress)
                : new MailAddress(settings.FromAddress, settings.FromName),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await smtpClient.SendMailAsync(mail);
    }

    private EmailSettings GetRequiredEmailSettings()
    {
        if (!TryGetEmailSettings(out var settings))
            throw new InvalidOperationException("Email settings are not fully configured");

        return settings;
    }

    private bool TryGetEmailSettings(out EmailSettings settings)
    {
        settings = default!;

        var smtpHost = _config["Email:SmtpHost"];
        var portValue = _config["Email:Port"];
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];
        var fromAddress =username;
        var fromName = _config["Email:FromName"];
        var enableSsl = bool.TryParse(_config["Email:EnableSsl"], out var parsedEnableSsl) ? parsedEnableSsl : true;

        if (string.IsNullOrWhiteSpace(smtpHost) ||
            string.IsNullOrWhiteSpace(portValue) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromAddress))
        {
            return false;
        }

        if (!int.TryParse(portValue, out var port))
            throw new InvalidOperationException("Email:Port must be a valid number");

        settings = new EmailSettings(
            smtpHost,
            port,
            username,
            password,
            fromAddress,
            fromName,
            enableSsl
        );

        return true;
    }

    private sealed record EmailSettings(
        string SmtpHost,
        int Port,
        string Username,
        string Password,
        string FromAddress,
        string? FromName,
        bool EnableSsl
    );
}
