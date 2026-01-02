using Domain.Enums;
using Domain.Entities;
using Infrastructure.Persistence;
using Application.Common.Interfaces.Services;

namespace Infrastructure.Services;

public class AuditService(DatabaseContext Context) : IAuditService
{
    public async Task LogAsync(AuditLog log)
    {
        await Context.AddAsync(log);
    }

    public Task LogEventAsync(AuthEventType eventType, 
        string? userId = null, 
        string? description = null, 
        Dictionary<string, string>? metadata = null)
    {
        var log = new AuditLog
        {
            UserId = userId!,
            EventType = eventType,
            EventName = eventType.ToString(),
            Description = description!,
            Metadata = metadata
        };

        return LogAsync(log);
    }
}
