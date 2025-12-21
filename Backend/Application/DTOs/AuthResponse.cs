using Domain.Entities;
using System;

namespace Application.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public UserInfo User { get; set; }
}
