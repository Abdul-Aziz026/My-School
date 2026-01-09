
using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    public DeleteUserCommandHandler(IUserRepository userRepository,
                                    ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
    }
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync<User>(request.UserId);
        if (user == null)
            throw new NotFoundException($"User with ID '{request.UserId}' not found");

        // Soft delete - just set IsActive to false
        //user.IsActive = false;
        //user.LockoutEnd = DateTime.UtcNow.AddYears(100); // Permanent lockout

        var isDeleteSuccessFull = await _userRepository.DeleteAsync<User>(user);
        if (!isDeleteSuccessFull)
        {
            throw new NotFoundException("db error");
        }
        await _cacheService.RemoveAsync($"UserInfo-{request.UserId}");
        return;

    }
}
