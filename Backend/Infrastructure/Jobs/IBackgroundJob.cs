namespace Infrastructure.Jobs;

public interface IBackgroundJob
{
    /// <summary>
    /// Execute the background job
    /// </summary>
    Task ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Job identifier for logging and monitoring
    /// </summary>
    string JobName { get; set; }
}
