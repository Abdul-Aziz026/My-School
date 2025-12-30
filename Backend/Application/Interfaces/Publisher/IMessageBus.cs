namespace Application.Interfaces.Publisher;

public interface IMessageBus
{
    // For in-process commands
    Task<TResponse> SendAsync<TRequest, TResponse>(TRequest command) where TRequest : class;

    // For async events
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
}
