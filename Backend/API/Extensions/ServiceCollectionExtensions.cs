using Application.Interfaces.Publisher;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
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

        // Application / infrastructure services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Email Services...
        services.AddScoped<IEmailService, BrevoEmailService>();

        services.AddScoped<IMessageBus, MessageBus>();

        return services;
    }
}
