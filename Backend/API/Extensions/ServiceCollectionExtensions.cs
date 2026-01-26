using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Interfaces;
using Domain.Repositories.Base;
using Infrastructure.DataMigrations;
using Infrastructure.DataMigrations.IndexDefinitions.Base;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Base;
using Infrastructure.Services;
using System.Reflection;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Index Definition Providers
        RegisterIndexDefinitionProviders(services);

        // Database 
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton<IDatabaseContext, DatabaseContext>();
        services.AddSingleton<IDatabaseIndexInitializer, DatabaseIndexInitializer>();
        services.AddSingleton<IAuditService, AuditService>();

        // unit of work
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddSingleton<IRepository, Repository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IClassRepository, ClassRepository>();
        services.AddSingleton<ISubjectRepository, SubjectRepository>();
        services.AddSingleton<IStudentRepository, StudentRepository>();
        services.AddSingleton<IEnrollmentRepository, EnrollmentRepository>();
        services.AddSingleton<IPaymentRepository, PaymentRepository>();

        // register JWT token service
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Email Services...
        services.AddScoped<IEmailService, BrevoEmailService>();

        // Message Bus for sending commands/events through(MediatR & RabbitMQ)
        services.AddScoped<IMessageBus, MessageBus>();

        // register cache service
        services.AddScoped<ICacheService, CacheService>();
        
        return services;
    }
    // sikho app
    // whats app:
    // 01345381211

    private static void RegisterIndexDefinitionProviders(IServiceCollection services)
    {
        services.AddSingleton<IIndexDefinitionProvider, StudentIndexDefinitions>();
        //var assembly = Assembly.GetExecutingAssembly();

        //var providerTypes = assembly.GetTypes()
        //    .Where(type =>
        //        typeof(IIndexDefinitionProvider).IsAssignableFrom(type) &&
        //        type.IsClass &&
        //        !type.IsAbstract &&
        //        type != typeof(BaseIndexDefinitionProvider)) // Exclude base class
        //    .ToList();

        //Console.WriteLine($"Found {providerTypes.Count} index definition providers to register:");

        //foreach (var type in providerTypes)
        //{
        //    services.AddSingleton(typeof(IIndexDefinitionProvider), type);
        //    Console.WriteLine($"  ✓ Registered: {type.Name}");
        //}
        //if (providerTypes.Count == 0)
        //{
        //    Console.WriteLine("  ⚠ WARNING: No index definition providers found!");
        //}
    }
}
