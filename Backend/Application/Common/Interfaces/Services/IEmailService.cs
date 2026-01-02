using Contracts.Events;
using System;

namespace Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toMail, string toName, string subject, string htmlContent);
    Task<bool> SendEmailAsync(SendEmailCommand command);
}
