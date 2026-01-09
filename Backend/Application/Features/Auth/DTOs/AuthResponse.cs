using Application.Features.Users.DTOs;

namespace Application.Features.Auth.DTOs;

public class AuthResponse
{
    public ResultStatus Status { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public UserDtoResponse? User { get; set; }
}

public enum ResultStatus
{
    Succeeded,
    Failed,
    InvalidToken,
    ExpiredToken,
    Revoked
}
