using System;

namespace Domain.Entities;

public class UserRefreshToken : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
}
