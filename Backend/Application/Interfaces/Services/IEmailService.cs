using System;

namespace Application.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toMail, string toName, string subject, string htmlContent);
    Task<bool> SendEmailWithAttachmentAsync(string toMail, string toName, string subject, string htmlContent, byte[] attachment, string attachmentName);
}
