
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync<User>(request.UserId);
        if (user == null)
            throw new Exception($"User with ID '{request.UserId}' not found");

        // Soft delete - just set IsActive to false
        user.IsActive = false;
        user.LockoutEnd = DateTime.UtcNow.AddYears(100); // Permanent lockout

        await _userRepository.UpdateAsync<User>(user);
        return;
    }
}
