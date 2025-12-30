using MediatR;
using MassTransit;
using Infrastructure.Services;
using Application.Interfaces.Services;

namespace API.Extensions;

public static class MasstransitAndMediatRExtensions
{
    public static IServiceCollection AddMediatRAndMasstransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Automatically scan and register all consumers
            //x.AddConsumersFromNamespaceContaining<OrderCreatedConsumer>();

            // Apply kebab-case naming for endpoints
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"], h =>
                {
                    h.Username(configuration["RabbitMQ:UserName"]!);
                    h.Password(configuration["RabbitMQ:Password"]!);
                });
                // This automatically creates a queue per consumer
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("MySchool", false));
            });
        });

        services.AddMediatR(typeof(Program).Assembly);
        return services;
    }
}
