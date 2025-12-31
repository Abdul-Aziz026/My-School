
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Auth.Commands.Logout;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand>
{
    private readonly IUserRepository _userRepository;
    public LogoutUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await _userRepository.RevokeRefreshTokenAsync(request.RefreshToken);
        }
        return;
    }
}
