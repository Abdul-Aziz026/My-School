using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var updatedUser = request;
        var currentUserId = request.UserId ?? throw new ArgumentNullException("User id not found.");
        var user = await _userRepository.GetByIdAsync<User>(currentUserId);
        if (user is null)
        {
            throw new NotFoundException("User not found");
        }
        await ApplyProfileUpdatesToUser(updatedUser, user);
        try
        {
            user.UpdatedAt = DateTime.UtcNow; var isUpdateSuccessfull = await _userRepository.UpdateAsync<User>(user);
            if (!isUpdateSuccessfull)
            {
                throw new NotFoundException("update failed");
            }
            return;
        }
        catch (Exception ex)
        {
            throw new NotFoundException($"update failed {ex.Message}");
        }
    }

    private static async Task ApplyProfileUpdatesToUser(UpdateUserCommand updatedUser, User user)
    {
        if (!string.IsNullOrWhiteSpace(updatedUser.UserName))
        {
            user.Email = updatedUser.UserName;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
        {
            user.Email = updatedUser.PhoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.ProfilePicture))
        {
            user.Email = updatedUser.ProfilePicture;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.Address))
        {
            user.Email = updatedUser.Address;
        }
    }
}
