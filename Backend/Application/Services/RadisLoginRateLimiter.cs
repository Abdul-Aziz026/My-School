public class RadisLoginRateLimiter : ILoginRateLimiter
{
    public async Task<bool> IsAllowedAsync(string key)
    {
        string attemptKey = $"login_attempt:{key}";
        string blockKey = $"login_block:{key}";

        //...
        throw new NotImplementedException() ;
    }

    public async Task ResetAsync(string key)
    {
        string attemptKey = $"login_attempt:{key}";
        string blockKey = $"login_block:{key}";

        //...
        throw new NotImplementedException();
    }
}
