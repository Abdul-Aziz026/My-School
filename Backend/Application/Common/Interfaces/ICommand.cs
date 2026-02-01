namespace Application.Common.Interfaces;


/// <summary>
/// Marker for fire-and-forget commands (void result).
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Marker for commands that return a result.
/// </summary>
public interface ICommand<TResponse>
{
}