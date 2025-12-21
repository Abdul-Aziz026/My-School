public interface ILoginRateLimiter
{
    Task<bool> IsAllowedAsync(string key);
    Task ResetAsync(string key);
}
