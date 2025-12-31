using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public class RegisterUserCommand : IRequest<RefreshTokenResponse>
{
    public RegisterDto RegisterDto { get; }
    public RegisterUserCommand(RegisterDto registerDto)
    {
        RegisterDto = registerDto;
    }

}
