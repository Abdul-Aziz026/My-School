using System;

namespace Application.Features.Auth.DTOs;

public class RefreshTokenResponse
{
    public ActionEvent Result { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public enum ActionEvent
{
    Succeeded,
    Failed,
    InvalidToken,
    ExpiredToken,
    Revoked
}
