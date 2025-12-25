using Application.Settings;

namespace API.Extensions;

public static class ConfigurationSettingExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthLockoutSettings>(configuration.GetSection("Auth:Lockout"));
        services.Configure<BrevoSettings>(configuration.GetSection("BrevoEmail"));
        return services;
    }
}
