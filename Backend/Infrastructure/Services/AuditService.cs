using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;

namespace Infrastructure.Services;

public class AuditService(DatabaseContext Context) : IAuditService
{

    public async Task LogAsync(AuditLog log)
    {
        await Context.AddAsync<AuditLog>(log);
    }

    public Task LogEventAsync(AuthActionEventType eventType, string? userId = null, string? description = null, IDictionary<string, string>? metadata = null)
    {
        throw new NotImplementedException();
    }
}
