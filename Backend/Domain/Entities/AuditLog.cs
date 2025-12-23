
using Domain.Enums;

namespace Domain.Entities;

public class AuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public AuthActionEventType EventType { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public Dictionary<string, string>? Metadata { get; set; }
}
