using Application.Common.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;

namespace API.MiddleWare;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    // paths you want to specially treat as auth events
    private static readonly string[] AuthPaths = new[] { "/api/auth/login", "/api/auth/register", "/api/auth/refresh", "/api/auth/logout" };
    
    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditService auditService)
    {
        var startTime = DateTime.UtcNow;
        var request = context.Request;
        var path = request.Path;
        var method = request.Method;
        string? ip = GetClientIpAddress(context);
        var userAgent = request.Headers["User-Agent"].ToString();
        var userId = context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        await _next(context);
        if (!AuthPathContains(path)) return;
        var statusCode = context.Response.StatusCode;
        var duration = DateTime.UtcNow - startTime;

        var authEvent = MapAuthEvent(path, statusCode);
        var metaData = new Dictionary<string, string?>
        {
            { "Path", path },
            { "Method", method },
            { "StatusCode", statusCode.ToString() },
            { "DurationMs", duration.TotalMilliseconds.ToString("F2") },
            { "IpAddress", ip },
            { "UserAgent", userAgent }
        };

        var log = new AuditLog
        {
            UserId = userId ?? "Anonymous",
            EventType = authEvent,
            EventName = authEvent.ToString(),
            Description = $"{authEvent} event at {path}",
            Metadata = metaData!,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await auditService.LogAsync(log);
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private bool AuthPathContains(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        foreach (var p in AuthPaths)
        {
            if (path.StartsWith(p, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private AuthEventType MapAuthEvent(string path, int statusCode)
    {
        // map by path and response code; 2xx => success, 4xx => failure/unauthorized
        if (path.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            return (statusCode >= 200 && statusCode < 300) ? AuthEventType.LoginSuccess : AuthEventType.LoginFailed;
        }

        if (path.StartsWith("/api/auth/register", StringComparison.OrdinalIgnoreCase))
        {
            return (statusCode >= 200 && statusCode < 300) ? AuthEventType.RegisterSuccess : AuthEventType.RegisterFailed;
        }

        if (path.StartsWith("/api/auth/refresh", StringComparison.OrdinalIgnoreCase))
        {
            return (statusCode >= 200 && statusCode < 300) ? AuthEventType.RefreshTokenIssued : AuthEventType.RefreshTokenRevoked;
        }

        if (path.StartsWith("/api/auth/logout", StringComparison.OrdinalIgnoreCase))
        {
            return AuthEventType.Logout;
        }

        // default
        return statusCode >= 200 && statusCode < 300 ? AuthEventType.LoginSuccess : AuthEventType.LoginFailed;
    }
}

public static class AuditMiddlewareExtensions
{
    public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditMiddleware>();
    }
}
