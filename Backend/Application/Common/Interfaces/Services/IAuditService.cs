using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(AuditLog log);
    Task LogEventAsync(AuthEventType eventType, string? userId = null, string? description = null, Dictionary<string, string>? metadata = null);
}