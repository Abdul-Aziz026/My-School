using Domain.Entities;
using System;

namespace Application.DTOs;

public class AuthResponse
{
    public ResultStatus Status { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public UserInfo User { get; set; }
}

public enum ResultStatus
{
    Succeeded,
    Failed
}
