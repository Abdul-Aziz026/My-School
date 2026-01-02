using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Repositories.Base;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Base;
using Infrastructure.Services;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Database adapter (keeps repository layer abstracted from driver)
        services.AddScoped<DatabaseContext>();

        // Repositories
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // register JWT token service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Email Services...
        services.AddScoped<IEmailService, BrevoEmailService>();

        // Message Bus for sending commands/events through(MediatR & RabbitMQ)
        services.AddScoped<IMessageBus, MessageBus>();

        // register cache service
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
