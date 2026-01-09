using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    public UpdateUserCommandHandler(IUserRepository userRepository,
                                    ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
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
            user.UpdatedAt = DateTime.UtcNow;
            var isUpdateSuccessfull = await _userRepository.UpdateAsync<User>(user);
            await _cacheService.SetAsync<User>($"UserInfo-{user.Id}", user);
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
            user.UserName = updatedUser.UserName;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
        {
            user.PhoneNumber = updatedUser.PhoneNumber;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.ProfilePicture))
        {
            user.ProfilePicture = updatedUser.ProfilePicture;
        }
        if (!string.IsNullOrWhiteSpace(updatedUser.Address))
        {
            user.Address = updatedUser.Address;
        }
    }
}
