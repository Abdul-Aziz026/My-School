using Application.Features.Users.Commands.CreateUser;
using Infrastructure.Consumers;
using MassTransit;

namespace API.Extensions;

public static class MasstransitAndMediatRExtensions
{
    public static IServiceCollection AddMediatRAndMasstransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Apply kebab-case naming for endpoints
            x.SetKebabCaseEndpointNameFormatter();
            // Automatically scan and register all consumers
            x.AddConsumersFromNamespaceContaining<NotifyStudentConsumer>();

            // register explicit consumer
            //x.AddConsumer<NotifyStudentConsumer>();

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
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateUserCommandHandler).Assembly);
        });
        return services;
    }
}
