
using Application.Common.Interfaces.Services;
using Contracts.Events;
using MassTransit;

namespace Infrastructure.Consumers;

public class EmailNotificationConsumer : IConsumer<SendEmailCommand>
{
    private readonly IEmailService _emailService;
    public EmailNotificationConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<SendEmailCommand> context)
    {
        var command = context.Message;
        var result = await _emailService.SendEmailAsync(command);

        if (!result)
        {
            // add retry confiuration for retry mechanism
            throw new Exception($"Failed to send email to {command.ToMail}");
        }
    }
}
