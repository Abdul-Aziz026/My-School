
using Domain.Enums;

namespace Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public AuthEventType EventType { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public Dictionary<string, string>? Metadata { get; set; }
}
