using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.Login;

public class LoginUserCommand : IRequest<RefreshTokenResponse>
{
    public LoginDto LoginDto { get; }
    public LoginUserCommand(LoginDto loginDto)
    {
        LoginDto = loginDto;
    }
}
