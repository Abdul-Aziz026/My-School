namespace Application.Interfaces.Publisher;

public interface IMessageBus
{
    // For in-process commands
    Task SendAsync<TRequest>(TRequest command) where TRequest : class;

    // For async events
    Task PublishAsync<T>(T command) where T: class;
}
