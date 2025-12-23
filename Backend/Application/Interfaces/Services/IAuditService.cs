using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(AuditLog log);
    Task LogEventAsync(AuthActionEventType eventType, string? userId = null, string? description = null, IDictionary<string, string>? metadata = null);
}