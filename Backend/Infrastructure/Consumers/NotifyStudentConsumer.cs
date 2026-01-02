using Application.Common.Interfaces.Services;
using Contracts.Events;
using MassTransit;

namespace Infrastructure.Consumers;

public class NotifyStudentConsumer : IConsumer<StudentNotifiedEvent>
{
    private readonly IEmailService _emailService;
    public NotifyStudentConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task Consume(ConsumeContext<StudentNotifiedEvent> context)
    {
        await _emailService.SendEmailAsync(
            context.Message.Email,
            context.Message.Name,
            context.Message.Subject,
            context.Message.Body
        );
    }
}
