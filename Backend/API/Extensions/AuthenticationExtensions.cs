using Application.Authorization;
using Infrastructure.Helper;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Bearer") // Use JWT Bearer tokens to authenticate requests
            // registers the JWT Bearer authentication handler
            .AddJwtBearer("Bearer", options =>
            {
                // check the commented section below for logging token validation events
                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                //        Log.Information($"OnMessageReceived - Token: {token?.Substring(0, 20)}...");
                //        return Task.CompletedTask;
                //    },
                //    OnAuthenticationFailed = context =>
                //    {
                //        Log.Error($"OnAuthenticationFailed: {context.Exception.GetType().Name} - {context.Exception.Message}");
                //        return Task.CompletedTask;
                //    },
                //    OnTokenValidated = context =>
                //    {
                //        Log.Information($"OnTokenValidated - User: {context.Principal?.Identity?.Name}");
                //        return Task.CompletedTask;
                //    },
                //    OnChallenge = context =>
                //    {
                //        Log.Warning($"OnChallenge - Error: {context.Error}, ErrorDescription: {context.ErrorDescription}");
                //        return Task.CompletedTask;
                //    }
                //};

                options.MapInboundClaims = false;
                // Configure token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // Validate the token issuer
                    ValidateAudience = true, // Validate the token audience
                    ValidateIssuerSigningKey = true, // Validate the signing key
                    ValidateLifetime = true,

                    ValidIssuer = ConfigurationHelper.GetConfigurationValue("Jwt:Issuer"), // Expected issuer
                    ValidAudience = ConfigurationHelper.GetConfigurationValue("Jwt:Audience"), // Expected audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationHelper.GetConfigurationValue("Jwt:SecretKey"))) // Signing key
                };
            });

        //services.AddAuthorization(options =>
        //{
        //    options.AddPolicy("CanViewUsers", policy =>
        //        //policy.Requirements.Add(new PermissionRequirement(Permissions.ViewUsers)));
        //        policy.Requirements.Add(new PermissionRequirement("CanViewUsersPermission")));
        //});

        services.AddAuthorization();

        return services;
    }
}
