using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace API.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Add custom rejection handler HERE
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.",cancellationToken);
            };

            // Global rate limiter: 1000 requests per minute with a queue of 100
            options.AddFixedWindowLimiter("GlobalLimiter", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(1);    // Time window of 1 minute
                opt.PermitLimit = 1000;                   // Allow 100 requests per minute
                opt.QueueLimit = 100;                      // Queue limit of 5
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // Policy 2: Login endpoint - 5 attempts per 1 minutes
            options.AddFixedWindowLimiter("login", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 2; // No queuing
            });

            // Policy 3: Registration endpoint - 3 attempts per hour
            options.AddFixedWindowLimiter("register", opt =>
            {
                opt.PermitLimit = 3;
                opt.Window = TimeSpan.FromHours(1);
                opt.QueueLimit = 0;
            });

            // Policy 4: API endpoint - 100 requests per minute
            options.AddSlidingWindowLimiter("api", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow = 6; // 10-second segments
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            });

        });

        return services;
    }
}
