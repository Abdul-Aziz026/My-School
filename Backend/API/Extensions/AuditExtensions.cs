using Application.Interfaces.Services;
using Infrastructure.Services;

namespace API.Extensions;

public static class AuditExtensions
{
    public static IServiceCollection AddAuditLogging(this IServiceCollection services)
    {
        services.AddScoped<IAuditService, AuditService>();
        return services;
    }
}
