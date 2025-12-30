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
    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest command) where TRequest : class
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        return await _mediator.Send<TResponse>((IRequest<TResponse>)command);
    }

    // Publish event asynchronously via MassTransit
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        await _publishEndpoint.Publish(@event);
    }
}
