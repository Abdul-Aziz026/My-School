using Application.Features.Users.Commands.CreateUser;
using Infrastructure.Consumers;
using MassTransit;

namespace API.Extensions;

public static class RedisExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });

        return services;
    }
}
