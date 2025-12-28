using Application.Interfaces.Services;
using Application.Settings;
using Microsoft.Extensions.Options;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace Infrastructure.Services;

public class BrevoEmailService : IEmailService
{
    private readonly TransactionalEmailsApi _apiInstance;
    private readonly BrevoSettings _settings;

    public BrevoEmailService(IOptions<BrevoSettings> settings)
    {
        _settings = settings.Value;

        Configuration.Default.ApiKey.Clear();
        Configuration.Default.ApiKey.Add("api-key", _settings.ApiKey);
        var config = new Configuration();
        config.ApiKey.Add("api-key", _settings.ApiKey);

        // Initialize API with the configuration
        _apiInstance = new TransactionalEmailsApi(config);
    }

    public async Task<bool> SendEmailAsync(string toMail, string toName, string subject, string htmlContent)
    {
        try
        {
            //toMail = "azizurcsebsmrstu@gmail.com";
            var sender = new SendSmtpEmailSender(_settings.SenderName, _settings.SenderEmail);
            var receiver = new SendSmtpEmailTo(toMail, toName);

            var sendSmtpEmail = new SendSmtpEmail(
                sender: sender,
                to: new List<SendSmtpEmailTo> { receiver },
                htmlContent: htmlContent,
                subject: subject
            );


            var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            Console.WriteLine($"Brevo email sent to {toMail}. Response id: {result?.MessageId}");
            return result is not null;
        }
        catch (Exception ex)
        {
            // Use proper logging instead of Console.WriteLine
            // _logger.LogError(ex, "Error sending email to {Email}", toMail);
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendEmailWithAttachmentAsync(
        string toMail, string toName, string subject,
        string htmlContent, byte[] attachment, string attachmentName)
    {
        try
        {
            var sender = new SendSmtpEmailSender(_settings.SenderName, _settings.SenderEmail);
            var receiver = new SendSmtpEmailTo(toMail, toName);

            var attachmentContent = new SendSmtpEmailAttachment(
                content: attachment,
                name: attachmentName
            );

            var sendSmtpEmail = new SendSmtpEmail(
                sender: sender,
                to: new List<SendSmtpEmailTo> { receiver },
                htmlContent: htmlContent,
                subject: subject,
                attachment: new List<SendSmtpEmailAttachment> { attachmentContent }
            );

            var result = await _apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            Console.WriteLine($"Brevo email sent to {toMail}. Response id: {result?.MessageId}");
            return result is not null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email with attachment: {ex.Message}");
            return false;
        }
    }
}

