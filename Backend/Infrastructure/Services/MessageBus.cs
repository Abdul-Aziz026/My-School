using Application.Interfaces.Publisher;
using MassTransit;
using MediatR;

namespace Infrastructure.Services;

public class MessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMediator _mediator;

    public MessageBus(IPublishEndpoint publishEndpoint, IMediator mediator)
    {
        _publishEndpoint = publishEndpoint;
        _mediator = mediator;
    }

    // Send command in-process via MediatR
    public async Task SendAsync<TRequest>(TRequest command) where TRequest : class
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        await _mediator.Send(command);
    }

    // Publish event asynchronously via MassTransit
    public async Task PublishAsync<T>(T command) where T : class
    {
        if (command is null)
            throw new ArgumentNullException();

        await _publishEndpoint.Publish(command);
    }
}
