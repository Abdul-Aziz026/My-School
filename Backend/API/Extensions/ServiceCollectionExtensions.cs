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

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Email Services...
        services.AddScoped<IEmailService, BrevoEmailService>();

        services.AddScoped<IMessageBus, MessageBus>();

        return services;
    }
}
