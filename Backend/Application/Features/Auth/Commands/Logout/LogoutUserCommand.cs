
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

public class LogoutUserCommand : IRequest
{
    public string? RefreshToken { get; set; }
    public LogoutUserCommand(string? refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
