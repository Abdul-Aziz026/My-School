using Application.Common.Interfaces.Publisher;
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

    public async Task PublishAsync<T>(T command) where T : class
    {
        if (command is null)
            throw new ArgumentNullException();

        await _publishEndpoint.Publish(command);
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest command)
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        return await _mediator.Send(command);
    }

    public async Task SendAsync<TCommand>(TCommand command) where TCommand : IRequest
    {
        await _mediator.Send(command);
    }
}
